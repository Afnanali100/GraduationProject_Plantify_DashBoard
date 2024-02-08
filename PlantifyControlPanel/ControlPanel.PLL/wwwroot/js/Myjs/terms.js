{
  
    function checkk(event) {
        var check = document.getElementById("check");
        if (check.checked != true) {
            alert("Agree our trems!");
            event.preventDefault();
        }
        
    }

}