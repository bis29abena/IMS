function ignoreSubmission(formId) {
    document.getElementById(`${formId}`).addEventListener("keydown", function (e) {
        if (e.key == "Enter") {
            e.preventDefault();
            return false
        }
    })
}