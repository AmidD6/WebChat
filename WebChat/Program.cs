using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=webchatdatabase;Trusted_Connection=True;";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessdenied";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/accessdenied", async (HttpContext context) =>
{
    context.Response.StatusCode = 403;
    await context.Response.WriteAsync("Access Denied");
});


app.MapGet("/login", async (HttpContext context) =>
{
    GetHtml(context);
    // html-форма для ввода логина/пароля
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/api/login", async (ApplicationContext db) => await db.Users.ToListAsync());

app.MapPost("/login", async (string? returnUrl, HttpContext context, ApplicationContext db) =>
{

    // получаем из формы email и пароль
    var form = context.Request.Form;
    // если email и/или пароль не установлены, посылаем статусный код ошибки 400

    if (!form.ContainsKey("username") || !form.ContainsKey("password"))
        return Results.BadRequest("Email и/или пароль не установлены");

    string name = form["username"];
    string password = form["password"];

    // находим пользователя 
    User? user = db.Users.FirstOrDefault(p => p.Name == name && p.Password == password);
    // если пользователь не найден, отправляем статусный код 401
    if (user is null) return Results.LocalRedirect("/login");
    var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, GetRole(user.Role))
    };
    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    await context.SignInAsync(claimsPrincipal);
    return Results.Redirect(returnUrl ?? "/");
});

// доступ только для роли admin
app.Map("/admin", [Authorize(Roles = "admin")] async (HttpContext context) =>
{
    GetHtml(context);
    await context.Response.SendFileAsync("wwwroot/admin.html");
});


app.MapGet("/api/datauser", [Authorize(Roles = "admin, moderator, user")] (HttpContext context) =>
{
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    var role = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
    var datauser = new List<string>() {
        login?.Value,
        role?.Value
    };
    return datauser;
});



// доступ только для ролей admin и user
app.Map("/", [Authorize(Roles = "admin, moderator, user")] async (HttpContext context) =>
{
    GetHtml(context);
    await context.Response.SendFileAsync("wwwroot/chatlist.html");

    /*return $"Name: {login?.Value}\nRole: {role?.Value}\n\n<a href='/logout'>Exit</a>";*/
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/login");
});
//--------------------------------------------Chat--------------------------------------------
app.MapGet("/api/chats", async (ApplicationContext db, HttpContext context) =>
{
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    return await db.Users.FromSqlRaw("SELECT * FROM Users WHERE Name != {0}", login?.Value).ToListAsync();
});

app.MapGet("/api/chats/{id:int}", async (int id, ApplicationContext db) =>
{
    User? user = await db.Users.FromSqlRaw("SELECT * FROM Users WHERE Id = {0}", id);

    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    return Results.Json(user);
});


//--------------------------------------------Admin Panel--------------------------------------------

app.MapGet("/api/admin/users", async (ApplicationContext db, HttpContext context) => {
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    return await db.Users.FromSqlRaw("SELECT * FROM Users WHERE Name != {0}", login?.Value).ToListAsync();
});

app.MapGet("/api/admin/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, отправляем его
    return Results.Json(user);
});

app.MapDelete("/api/admin/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, удаляем его
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/admin/users", async (User user, ApplicationContext db) =>
{
    // добавляем пользователя в массив
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});

app.MapPut("/api/admin/users", async (User userData, ApplicationContext db) =>
{
    // получаем пользователя по id
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userData.Id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
    user.Password = userData.Password;
    user.Name = userData.Name;
    user.Role = userData.Role;
    await db.SaveChangesAsync();
    return Results.Json(user);
});


/*app.MapGet("/api/users/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, отправляем его
    return Results.Json(user);
});

app.MapPost("/api/users", async (User user, ApplicationContext db) =>
{
    // добавляем пользователя в массив
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return user;
});*/

app.Run();

string GetRole(int role)
{
    if (role == 1) return "admin";
    else if (role == 2) return "moderator";
    else if (role == 3) return "user";
    else return "unknow";
}

void GetHtml(HttpContext context)
{
    context.Response.ContentType = "text/html; charset=utf-8";
}