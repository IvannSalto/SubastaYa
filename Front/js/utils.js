export const formatPrice = (amount) => '$ ' + amount.toLocaleString('es-AR');

export const parsePrice = (priceStr) => parseInt(priceStr.replace(/[^0-9]/g, ''), 10) || 0;

export const showToast = (message, isError = false) => {
    let toast = document.getElementById('toast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toast';
        toast.className = 'toast-notification';
        document.body.appendChild(toast);
    }
    toast.textContent = message;
    toast.className = `toast-notification show ${isError ? 'error' : ''}`;
    setTimeout(() => toast.classList.remove('show'), 3500);
};