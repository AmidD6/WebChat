async function Auth() {
    const respons = await fetch("/api/login", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    const persons = await respons.json();

    document.getElementById("loginBtn").addEventListener('click', async (e) => {
        var username = document.getElementById("userName");
        var password = document.getElementById("userPass");
        var errorinput = document.getElementById("errorInputs");

        var check = false;
        for (var person of persons) {
            if (username.value == person.name && password.value == person.password) {
                check = true;
                break;
            }
        }
        if (!check) {
            errorinput.style.display = "block";
            e.preventDefault();
        } else errorinput.style.display = "none";
    })
}

Auth();




/*async function getUsers() {
    // отправляет запрос и получаем ответ
    const response = await fetch("/api/users", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    // если запрос прошел нормально
    if (response.ok === true) {
        console.log(1);
        // получаем данные
        const users = await response.json();
        const rows = document.querySelector("tbody");
        // добавляем полученные элементы в таблицу
        users.forEach(user => rows.append(row(user)));
    }
    else {
        console.log(123);
    }
}

function row(user) {

    const tr = document.createElement("tr");
    const idTd = document.createElement("td");

    tr.setAttribute("data-rowid", user.id);
    idTd.append(user.id)
    tr.append(idTd);

    const nameTd = document.createElement("td");
    nameTd.append(user.name);
    tr.append(nameTd);

    const passTd = document.createElement("td");
    passTd.append(user.password);
    tr.append(passTd);

    const roleTd = document.createElement("td");
    roleTd.append(user.role);
    tr.append(roleTd);

    return tr;
}


// загрузка пользователей
getUsers();*/