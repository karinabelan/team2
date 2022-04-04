function OnCustomerChanged(executionContext) {
    var formContext = executionContext.getFormContext();
    var user = formContext.getAttribute("t2_customer").getValue();
    if (user) {
        var userName = user[0].name;
        formContext.getAttribute("t2_name").setValue(userName);
    }

}