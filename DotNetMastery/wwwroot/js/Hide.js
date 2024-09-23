jQuery(function () {
    jQuery("Input_Role").on('change', function () {
        var selection = jQuery("Input.Role Option:Selected").text();
        if (selection == "Company") {
            jQuery("Input_CompanyId").show();
        }
        else {
            jQuery("Input_CompanyId").hide();
        }
    }).trigger('change');
});