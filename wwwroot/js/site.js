const smallScreen = window.matchMedia('(max-width: 1200px)');

function handleScreenChange() {
	const leftColumn = document.getElementById("left-column");
	const middleColumn = document.getElementById("middle-column");
	const rightColumn = document.getElementById("right-column");

	if (smallScreen.matches) {
		leftColumn.style.display = "none";
		rightColumn.style.display = "none";

		leftColumn.style.order = "0";
		middleColumn.style.order = "1";
		rightColumn.style.order = "0";

		leftColumn.style.height = "55vh";
		middleColumn.style.height = "55vh";
		rightColumn.style.height = "55vh";
	} else {
		leftColumn.style.display = "block";
		rightColumn.style.display = "block";

		leftColumn.style.order = "0";
		middleColumn.style.order = "0";
		rightColumn.style.order = "0";

		leftColumn.style.height = "70vh";
		const pagePath = window.location.pathname;
		const header = document.getElementsByTagName("header")[0];
		const logo = document.getElementById("logo");
		if (pagePath.startsWith("/Author/ReadChapter") || pagePath.startsWith("/Reader/ViewChapter"))
		{
			header.style.height = "9vh";
			logo.style.height = "5vh";
			middleColumn.style.height = "85vh";
		}
		else
		{
			header.style.height = "20vh";
			logo.style.height = "15vh";
			middleColumn.style.height = "70vh";
		}
		rightColumn.style.height = "70vh";
	}
}

smallScreen.addListener(handleScreenChange);

handleScreenChange();

function OpenLogin() {
	document.getElementById("left-column").style.display = "block";
	document.getElementById("right-column").style.display = "none";

	document.getElementById("login-button").style.display = "none";
	document.getElementById("login-close-button").style.display = "block";

	document.getElementById("search-button").style.display = "block";
	document.getElementById("search-close-button").style.display = "none";
}

function CloseLogin() {
	document.getElementById("left-column").style.display = "none";
	document.getElementById("right-column").style.display = "none";

	document.getElementById("login-button").style.display = "block";
	document.getElementById("login-close-button").style.display = "none";

	document.getElementById("search-button").style.display = "block";
	document.getElementById("search-close-button").style.display = "none";
}

function OpenSearch() {
	document.getElementById("left-column").style.display = "none";
	document.getElementById("right-column").style.display = "block";

	document.getElementById("login-button").style.display = "block";
	document.getElementById("login-close-button").style.display = "none";

	document.getElementById("search-button").style.display = "none";
	document.getElementById("search-close-button").style.display = "block";
}

function CloseSearch() {
	document.getElementById("left-column").style.display = "none";
	document.getElementById("right-column").style.display = "none";

	document.getElementById("login-button").style.display = "block";
	document.getElementById("login-close-button").style.display = "none";

	document.getElementById("search-button").style.display = "block";
	document.getElementById("search-close-button").style.display = "none";
}

function GetMenu(IsLogged) {
	const xhttp = new XMLHttpRequest();

	xhttp.onload = function () {
		document.getElementById("left-column").firstElementChild.innerHTML = xhttp.responseText;
	}

	let controller = "/Navigation/";
	if (IsLogged) { controller += "MenuPartial"; }
	else { controller += "LoginPartial"; }
	xhttp.open("GET", controller, true);
	xhttp.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
	xhttp.send();
}

function GetAdminMenu() {
	const xhttp = new XMLHttpRequest();
	xhttp.onload = function () {
		const divElement = document.createElement("div");
		divElement.classList.add("tab");
		divElement.classList.add("left");
		divElement.classList.add("flex-column");
		divElement.classList.add("std-pad-2");
		const leftColumn = document.getElementById("left-column");
		leftColumn.appendChild(divElement);
		leftColumn.lastElementChild.innerHTML = xhttp.responseText;
	}
	xhttp.open("GET", "/Navigation/AdminMenuPartial", true);
	xhttp.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
	xhttp.send();
}

function GetSearch() {
	const xhttp = new XMLHttpRequest();
	xhttp.onload = function () {
		const rightColumn = document.getElementById("right-column");
		rightColumn.innerHTML += xhttp.responseText;
	}
	xhttp.open("GET", "/Navigation/SearchPartial", true);
	xhttp.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
	xhttp.send();
}