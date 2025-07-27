function MoveUp(event) {
	console.log("up");
	const button = event.currentTarget;
	console.log(button);
	const page = button.closest('.page');
	console.log(page);
	const prevPage = page.previousElementSibling;
	console.log(prevPage);
	console.log("up");

	if (prevPage && prevPage.classList.contains('page')) {
		page.parentNode.insertBefore(page, prevPage);
		updateOrders();
		reindexPages();
	}
}

function MoveDown(event) {
	console.log("down");
	const button = event.currentTarget;
	const page = button.closest('.page');
	const nextPage = page.nextElementSibling;

	if (nextPage && nextPage.classList.contains('page')) {
		page.parentNode.insertBefore(nextPage, page);
		updateOrders();
		reindexPages();
	}
}

function DeletePage(event) {
	const button = event.currentTarget;
	const page = button.closest('.page');
	page.remove();
	reindexPages();
}

document.querySelectorAll('.move-up').forEach(button => {
	button.addEventListener('click', MoveUp);
});

document.querySelectorAll('.move-down').forEach(button => {
	button.addEventListener('click', MoveDown);
});

document.querySelectorAll('.delete').forEach(button => {
	button.addEventListener('click', DeletePage);
});

function renderPreviews(preview) {
	files.forEach(file => {
		const reader = new FileReader();
		reader.onload = (e) => {
			const div = document.createElement("div");
			div.className = "preview-item";

			const img = document.createElement("img");
			img.src = e.target.result;
			img.style.width = "100%";
			img.style.objectFit = "cover";
			img.style.aspectRatio = "1/1";

			div.appendChild(img);
			preview.appendChild(div);
		};
		reader.readAsDataURL(file);
	});
}

function AddPageCommon(pageDiv, pageName) {
	const buttonSpan = document.createElement("span");
	pageDiv.appendChild(buttonSpan);

	const moveUpButton = document.createElement("button");
	moveUpButton.type = 'button';
	moveUpButton.classList.add("move-up");
	moveUpButton.classList.add("btn");
	moveUpButton.classList.add("std-pad");
	moveUpButton.innerHTML = "Up";
	buttonSpan.appendChild(moveUpButton);
	moveUpButton.addEventListener('click', MoveUp);

	const moveDownButton = document.createElement("button");
	moveDownButton.type = 'button';
	moveDownButton.classList.add("move-down");
	moveDownButton.classList.add("btn");
	moveDownButton.classList.add("std-pad");
	moveDownButton.innerHTML = "Down";
	buttonSpan.appendChild(moveDownButton);
	moveDownButton.addEventListener('click', MoveDown);

	const imageSpan = document.createElement("span");
	pageDiv.appendChild(imageSpan);

	const imageInput = document.createElement("input");
	imageInput.classList.add("btn");
	imageInput.classList.add("std-pad");
	imageInput.type = "file";
	imageInput.name = pageName;
	imageInput.accept = ".jpg, .png, .gif";
	imageSpan.appendChild(imageInput);

	const preview = document.createElement("div");
	imageSpan.appendChild(preview);
	imageInput.addEventListener("change", () => {
		preview.innerHTML = "";
		files = Array.from(imageInput.files);
		renderPreviews(preview);
	});

	const deleteButton = document.createElement("button");
	deleteButton.type = 'button';
	deleteButton.classList.add("delete");
	deleteButton.classList.add("btn");
	deleteButton.classList.add("std-pad");
	deleteButton.innerHTML = "Delete";
	imageSpan.appendChild(deleteButton);
	deleteButton.addEventListener('click', DeletePage);
}