import { formatPrice, parsePrice, showToast } from '../utils.js';
import { placeBid } from '../auctionsService.js';

export function initBidManager(onBidSuccess) {
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

            const userObj = JSON.parse(session);
            const saldo = userObj.wallet !== undefined ? userObj.wallet : 0;
            document.getElementById('bidModalWalletBalance').textContent = `$ ${Number(saldo).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
            
            activeCardElement = card;
            currentActivePrice = parsePrice(card.querySelector('.price-amount')?.textContent || '$ 0');

            const minBid = currentActivePrice + 1000;

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
        bidForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const enteredBid = parseInt(document.getElementById('bidAmount').value, 10);
            const submitBtn = bidForm.querySelector('button[type="submit"]');

            if (isNaN(enteredBid) || enteredBid <= currentActivePrice) {
                showToast(`Tu oferta debe ser mayor a ${formatPrice(currentActivePrice)}`, true);
                return;
            }

            if (activeCardElement) {
                const auctionId = activeCardElement.getAttribute('data-id');

                try {
                    if (submitBtn) submitBtn.disabled = true;
                    
                    await placeBid(auctionId, enteredBid);

                    // Si el backend responde OK, actualizamos el precio en la interfaz
                    activeCardElement.querySelector('.price-amount').textContent = formatPrice(enteredBid);
                    activeCardElement.setAttribute('data-price', enteredBid);
                    currentActivePrice = enteredBid;

                    showToast(`¡Puja realizada con éxito por ${formatPrice(enteredBid)}!`);
                    closeBidModalWindow();

                    if (typeof onBidSuccess === 'function') {
                        onBidSuccess();
                    }

                } catch (error) {
                    showToast(error.message || 'Ocurrió un error al procesar tu puja.', true);
                } finally {
                    if (submitBtn) submitBtn.disabled = false;
                }
            }
        });
    }

    window.addEventListener('click', (e) => { if (e.target === bidModal) closeBidModalWindow(); });
    document.addEventListener('keydown', (e) => { if (e.key === 'Escape' && bidModal?.classList.contains('active')) closeBidModalWindow(); });
}