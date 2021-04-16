// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.js-update-customer').on('click',
    (event) => {
        debugger;
        let firstName = $('.js-first-name').val();
        let lastName = $('.js-last-name').val();
        let customerId = $('.js-customer-id').val();

        console.log(`${firstName} ${lastName}`);

        let data = JSON.stringify({
            firstName: firstName,
            lastName: lastName
        });

        // ajax call
        let result = $.ajax({
            url: `/customer/${customerId}`,
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            console.log('Update was successful');
            // success
        }).fail(failure => {
            // fail
            console.log('Update failed');
        });
    });

$('.js-customers-list tbody tr').on('click',
    (event) => {
        console.log($(event.currentTarget).attr('id'));
    });

$('#exampleModal_trDataRow').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var row = button.parents("tr");

    var IBAN = row.find(".IBAN").text();
    var Description = row.data('.Description');
    var Currency = row.data('.Currency');
    var State = row.data('.State');

});

$('#exampleModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var IBAN = button.attr('data-IBAN');
    var Description = button.attr('data-Description');
    var Currency = button.attr('data-Currency');
    var State = button.attr('data-State');

    var modal = $(this);
    modal.find('.modal-body #IBAN').val(IBAN);
    modal.find('.modal-body #Description').val(Description);
    modal.find('.modal-body #Currency').val(Currency);
    modal.find('.modal-body #State').val(State);
});


$('.UpdateState').on('click',
    (event) => {
        var accountId = $('#IBAN').val();
        var State = parseInt($(".AccountStateCombo option:selected").val());

        let data = JSON.stringify({
            AccountId: accountId,
            State: State
        });

        // ajax call
        let result = $.ajax({
            url: `/account/${accountId}`,
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            console.log('Update was successful');
            // success
        }).fail(failure => {
            // fail
            console.log('Update failed');
        });
});


$('.js-pay').on('click',
    (event) => {
        let cardnumber = $('.js-card-number').val();
        let expirationmonth = parseInt($('.js-expiration-month').val());
        let expirationyear = parseInt($('.js-expiration-year').val());
        let amount = parseFloat($('.js-amount').val());

        //console.log(`${js-card-number} ${lastName}`);

        let data = JSON.stringify({
            cardnumber: cardnumber,
            expirationmonth: expirationmonth,
            expirationyear: expirationyear,
            amount: amount
        });

        //1111222233334444
        //1111222233335555
        $(".js-card-number").attr('disabled', 'disabled');
        $(".js-expiration-month").attr('disabled', 'disabled');
        $(".js-expiration-year").attr('disabled', 'disabled');
        $(".js-amount").attr('disabled', 'disabled');
        $(".js-pay").attr('disabled', 'disabled');
        debugger;
        // ajax call
        let result = $.ajax({
            url: `/card/checkout`,
            method: 'POST',
            contentType: 'application/json',
            data: data
        }).done(response => {
            console.log('Update was successful');
            $('#js-card-form').hide();
            $('#js-alert-success').show();
        }).fail(failure => {
            console.log('Update failed');
            $('#js-card-form').hide();
            $('#js-alert-danger').show();
        });
    });