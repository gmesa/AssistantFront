export function scrollToBottom(idElement) {
    var objDiv = document.getElementById(idElement);
    objDiv.scrollTop = objDiv.scrollHeight + 200;
    //objDiv.scrollIntoView({ block: 'end', behavior: 'smooth' })
}