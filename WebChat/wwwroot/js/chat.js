async function getUsersChat() {
    const respons = await fetch("/api/chats", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    var ind = 0;
    const data = await respons.json();
    for (var user in data) {
        var div = document.createElement('div');
        var i = document.createElement('i');
        var p = document.createElement('p');
        p.textContent = data[user].name;
        div.className = 'UserLink bg-info shadow-sm mb-4 rounded';
        i.className = "fa-solid fa-folder";
        div.id = 'UserID';
        div.setAttribute('data-num', data[user].id);
        div.setAttribute('data-role', data[user].role);
        div.prepend(i);
        div.append(p);
        listUser.append(div);
    }

    var divs = listUser.querySelectorAll('#UserID');
    for (var item = 0; item < divs.length; item++) {
        divs[item].addEventListener('click', function () {
            getMessage(this.dataset.num, this.textContent, this.dataset.role);
            console.log(this.textContent);
        })
    }

}

async function getMessage(id, userName, userRole) {

    const response = await fetch(`/api/chats/${id}`, {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const user = await response.json();
        chatHead.style.display = "block";
        headUserName.textContent = userName;
        headUserRole.textContent = getRole(userRole);

        console.log(user);
    }

}



var getRole = (role) => {
    if (role == 1) return "Admin";
    else if (role == 2) return "Moderator";
    else if (role == 3) return "User";
    else "Not role";
};

getUsersChat();