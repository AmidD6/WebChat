async function getUsers() {
    // отправляет запрос и получаем ответ
    const response = await fetch("/api/admin/users", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const users = await response.json();
        const rows = document.querySelector("tbody");
        // добавляем полученные элементы в таблицу
        users.forEach(user => rows.append(row(user)));
    }
}

async function getUser(id) {
    const response = await fetch(`/api/admin/users/${id}`, {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const user = await response.json();
        document.getElementById("userId").value = user.id;
        document.getElementById("userName").value = user.name;
        document.getElementById("userPass").value = user.password;
        document.getElementById("userRole").value = user.role;
    }
    else {
        // если произошла ошибка, получаем сообщение об ошибке
        const error = await response.json();
        console.log(error.message); // и выводим его на консоль
    }
}

async function createUser(userName, userPass, userRole) {

    const response = await fetch("api/admin/users", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: userName,
            password: userPass,
            role: userRole
        })
    });
    if (response.ok === true) {
        const user = await response.json();
        document.querySelector("tbody").append(row(user));
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}


async function editUser(userId, userName, userPass, userRole) {
    const response = await fetch("api/admin/users", {
        method: "PUT",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            id: userId,
            name: userName,
            password: userPass,
            role: userRole
        })
    });
    if (response.ok === true) {
        const user = await response.json();
        document.querySelector(`tr[data-rowid='${user.id}']`).replaceWith(row(user));
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

async function deleteUser(id) {
    const response = await fetch(`/api/admin/users/${id}`, {
        method: "DELETE",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const user = await response.json();
        document.querySelector(`tr[data-rowid='${user.id}']`).remove();
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}


function row(user) {

    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", user.id);

    const idTd = document.createElement("td");
    idTd.append(user.id);
    tr.append(idTd);

    const nameTd = document.createElement("td");
    nameTd.append(user.name);
    tr.append(nameTd);

    const passTd = document.createElement("td");
    passTd.append(user.password);
    tr.append(passTd);

    const roleTd = document.createElement("td");
    roleTd.append(getRole(user.role));
    tr.append(roleTd);

    const linkPutTd = document.createElement("td");
    const linkDelTd = document.createElement("td");

    const editLink = document.createElement("button");
    editLink.innerHTML = '<i class="fa-solid fa-pen-to-square"></i>';
    editLink.addEventListener("click", async () => await getUser(user.id));
    editLink.className = "btn btn-warning";
    linkPutTd.append(editLink);

    const removeLink = document.createElement("button");
    removeLink.innerHTML = '<i class="fa-sharp fa-solid fa-user-minus"></i>';
    removeLink.className = "btn btn-danger";
    removeLink.addEventListener("click", async () => await deleteUser(user.id));

    linkDelTd.append(removeLink);
    tr.appendChild(linkPutTd);
    tr.appendChild(linkDelTd);


    return tr;
}


function reset() {
    document.getElementById("userId").value =
    document.getElementById("userName").value =
    document.getElementById("userPass").value = "";
}
document.getElementById("resetBtn").addEventListener("click", () => reset());

document.getElementById("saveBtn").addEventListener("click", async () => {

    const id = document.getElementById("userId").value;
    const name = document.getElementById("userName").value;
    const pass = document.getElementById("userPass").value;
    const role = document.getElementById("userRole").value;
    if (id === "")
        await createUser(name, pass, role);
    else
        await editUser(id, name, pass, role);
    reset();
});

var getRole = (role) => {
    if (role == 1) return "Admin";
    else if (role == 2) return "Moderator";
    else if (role == 3) return "User";
    else "Not role";
};


getUsers();