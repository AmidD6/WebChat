async function Auth() {
    const response = await fetch("/api/datauser", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    const data = await response.json();
    document.getElementById('getAdmin').textContent = data[0];
    var button = document.createElement('a');
    button.href = "/admin";
    button.classList.add('btn-success');
    button.classList.add('btn');
    button.textContent = 'Адмін панель';


    if (data[1] == 'admin') AddAdminPanel.prepend(button);
}

Auth();