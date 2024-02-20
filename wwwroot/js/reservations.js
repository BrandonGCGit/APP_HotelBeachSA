function changeToPayment() {
    event.preventDefault();
    document.getElementById('dates').classList.toggle('d-none');
    document.getElementById('payment').classList.toggle('d-none');

    document.getElementById('dates-circle').classList.toggle('circle');
    document.getElementById('dates-circle').classList.toggle('circle-outline');

    document.getElementById('payment-circle').classList.toggle('circle');
    document.getElementById('payment-circle').classList.toggle('circle-outline');
}

function changeToConfirmation() {
    event.preventDefault();
    document.getElementById('payment').classList.toggle('d-none');
    document.getElementById('confirmation').classList.toggle('d-none');
    document.getElementById('summary').style.display = 'none';

    document.getElementById('payment-circle').classList.toggle('circle');
    document.getElementById('payment-circle').classList.toggle('circle-outline');

    document.getElementById('confirmation-circle').classList.toggle('circle');
    document.getElementById('confirmation-circle').classList.toggle('circle-outline');
}

function showCheckInfo() {
    var componentSelect = document.getElementById('type-payment');
    var selectedOption = componentSelect.value;


    switch (selectedOption) {
        case '1':
            document.getElementById('card-payment').style.display = 'none';
            document.getElementById('check-payment').style.display = 'none';
            break
        case '2':
            document.getElementById('card-payment').style.display = 'block';
            document.getElementById('check-payment').style.display = 'none';
            break
        case '3':
            document.getElementById('check-payment').style.display = 'block';
            document.getElementById('card-payment').style.display = 'none';
            break
    }


}

