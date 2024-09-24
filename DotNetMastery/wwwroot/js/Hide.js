jQuery(function () {
    $("#Input_Role").on('change', function () {
        var box = document.getElementById('Input_Role');
        var selection = box.options[box.selectedIndex].text;
        //var selection = $('#Input.Role').find("option:selected").text();
        if (selection == 'Company') {
            $('#Input_CompanyId').show();
        }
        else {
            $('#Input_CompanyId').hide();
        }
    })
})