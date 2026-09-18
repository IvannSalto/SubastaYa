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

export const getCurrentUserId = () => {
    const token = localStorage.getItem('token');
    if (!token) return null;
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
        const payload = JSON.parse(jsonPayload);
        
        const nameId = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid || payload.sub || payload.id;
        return nameId ? parseInt(nameId, 10) : null;
    } catch (error) {
        return null;
    }
};