import { formatPrice, parsePrice, showToast } from '../utils.js';
import { placeBid } from '../auctionsService.js';
export function initBidManager() {
    const bidModal = document.getElementById('bidModal');
    const closeBidBtn = document.getElementById('closeBidBtn');
    const bidForm = document.getElementById('bidForm');

    let activeCardElement = null;
    let currentActivePrice = 0;

    const closeBidModalWindow = () => {
        if (bidModal) {
            bidModal.classList.remove('active');
            activeCardElement = null;
            if (bidForm) bidForm.reset();
        }
    };

    document.addEventListener('click', (e) => {
        if (e.target && e.target.classList.contains('btn-submit') && !e.target.hasAttribute('disabled')) {
            e.preventDefault();
            const card = e.target.closest('.auction-card');
            if (!card) return;

            const session = localStorage.getItem('currentUser');
            if (!session) {
                showToast('Debes iniciar sesión para realizar una puja', true);
                document.getElementById('loginModal')?.classList.add('active');
                return;
            }

            activeCardElement = card;
            currentActivePrice = parsePrice(card.querySelector('.price-amount')?.textContent || '$ 0');
            const minBid = currentActivePrice + 10000;

            document.getElementById('bidItemTitle').textContent = card.querySelector('.card-title')?.textContent;
            document.getElementById('bidCurrentPrice').textContent = formatPrice(currentActivePrice);
            document.getElementById('bidAmount').value = minBid;
            document.getElementById('bidAmount').min = minBid;
            document.getElementById('bidMinHint').textContent = `Monto mínimo sugerido: ${formatPrice(minBid)}`;

            bidModal.classList.add('active');
        }
    });

    if (closeBidBtn) closeBidBtn.addEventListener('click', closeBidModalWindow);

    if (bidForm) {
        bidForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const enteredBid = parseInt(document.getElementById('bidAmount').value, 10);

            if (isNaN(enteredBid) || enteredBid <= currentActivePrice) {
                showToast(`Tu oferta debe ser mayor a ${formatPrice(currentActivePrice)}`, true);
                return;
            }

            if (activeCardElement) {
                activeCardElement.querySelector('.price-amount').textContent = formatPrice(enteredBid);
                activeCardElement.setAttribute('data-price', enteredBid);
            }

            showToast(`¡Puja realizada con éxito por ${formatPrice(enteredBid)}!`);
            closeBidModalWindow();
        });
    }

    window.addEventListener('click', (e) => { if (e.target === bidModal) closeBidModalWindow(); });
    document.addEventListener('keydown', (e) => { if (e.key === 'Escape' && bidModal?.classList.contains('active')) closeBidModalWindow(); });
}