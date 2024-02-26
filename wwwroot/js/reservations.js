function changeToPayment() {
    event.preventDefault();
    document.getElementById('dates').classList.toggle('d-none');
    document.getElementById('payment').classList.toggle('d-none');

    document.getElementById('dates-circle').classList.toggle('circle');
    document.getElementById('dates-circle').classList.toggle('circle-outline');

    document.getElementById('payment-circle').classList.toggle('circle');
    document.getElementById('payment-circle').classList.toggle('circle-outline');
}

function changeToDates() {
    event.preventDefault();
    document.getElementById('dates').classList.toggle('d-none');
    document.getElementById('payment').classList.toggle('d-none');

    document.getElementById('payment-circle').classList.toggle('circle');
    document.getElementById('payment-circle').classList.toggle('circle-outline');

    document.getElementById('dates-circle').classList.toggle('circle');
    document.getElementById('dates-circle').classList.toggle('circle-outline');
}

function showCheckInfo() {
    var componentSelect = document.getElementById('type-payment');
    var selectedOption = componentSelect.value;


    switch (selectedOption) {
        case 'K':
            document.getElementById('card-payment').style.display = 'none';
            document.getElementById('check-payment').style.display = 'none';
            break
        case 'T':
            document.getElementById('card-payment').style.display = 'block';
            document.getElementById('check-payment').style.display = 'none';
            break
        case 'C':
            document.getElementById('check-payment').style.display = 'block';
            document.getElementById('card-payment').style.display = 'none';
            break
    }


}

