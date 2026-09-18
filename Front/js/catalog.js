import { getFilteredAuctions } from './auctionsService.js';
import { formatPrice } from './utils.js';
import { startCountdown } from './components/timer.js';
import { initBidManager } from './components/bidManager.js';
import { initAuctionCreator } from './components/auctionCreator.js';

document.addEventListener('DOMContentLoaded', () => {
    const auctionGrid = document.getElementById('auction-grid');
    const searchInput = document.getElementById('searchInput');
    const categoryFilter = document.getElementById('categoryFilter');
    const statusFilter = document.getElementById('statusFilter');
    const minPriceInput = document.getElementById('minPrice');
    const maxPriceInput = document.getElementById('maxPrice');
    const sortBySelect = document.getElementById('sortBy');

    const renderCards = (auctions) => {
        auctionGrid.innerHTML = '';
        if (auctions.length === 0) {
            auctionGrid.innerHTML = '<p style="color:var(--text-muted); grid-column: 1/-1; text-align:center;">No se encontraron subastas.</p>';
            return;
        }

        auctions.forEach(subasta => {
            const stateDictionary = {
                'Active': { text: 'En Curso', cssClass: 'activa' },
                'Active': { text: 'En Curso', cssClass: 'activa' },
                'Pending': { text: 'Próximamente', cssClass: 'proxima' },
                'Closed': { text: 'Finalizada', cssClass: 'finalizada' },
                'FinishedWithNoWinner': { text: 'Desierta', cssClass: 'finalizada' }
            };
            
            const mappedState = stateDictionary[subasta.state] || { text: subasta.state, cssClass: 'finalizada' };
            
            const isInactive = subasta.state !== 'Active';
            const btnText = subasta.state === 'Pending' ? 'Próximamente' : (isInactive ? 'Subasta Finalizada' : 'Pujar Ahora');

            const cardHTML = `
            <article class="auction-card" data-state="${subasta.state}" data-price="${subasta.basePrice}" data-end-date="${subasta.endDate}">
                <div class="card-image-wrapper">
                    <img src="${subasta.urlImage}" alt="${subasta.title}">
                    
                    <span class="badge badge-${mappedState.cssClass}">${mappedState.text}</span>  
                </div>
                    <div class="card-body">
                        <h3 class="card-title">${subasta.title}</h3>
                        <p class="card-location">${subasta.description.substring(0, 50)}...</p>
                        <div class="card-time-info" style="font-size:0.85rem; margin:0.5rem 0; font-weight:600;">
                            <span class="time-countdown">Calculando...</span>
                        </div>
                        <div class="card-price-info">
                            <span class="price-label">Precio Actual:</span>
                            <span class="price-amount">${formatPrice(subasta.basePrice)}</span>
                        </div>
                        <button class="btn-submit" ${isInactive ? 'disabled' : ''}>${btnText}</button>
                    </div>
                </article>
            `;
            auctionGrid.innerHTML += cardHTML;
        });

        startCountdown(); // Arranca el reloj
    };

    const applyFiltersAndSort = async () => {
        const state = statusFilter?.value || 'all';
        const categoryId = categoryFilter?.value || 'all';
        const sortBy = sortBySelect?.value || 'default';
        const query = searchInput?.value.toLowerCase().trim() || '';
        const minPrice = minPriceInput?.value ? parseFloat(minPriceInput.value) : null;
        const maxPrice = maxPriceInput?.value ? parseFloat(maxPriceInput.value) : null;

        let realData = await getFilteredAuctions(state, categoryId, sortBy, query, minPrice, maxPrice);

        renderCards(realData);
    };

    let timeoutId;
    const debouncedApplyFilters = () => {
        clearTimeout(timeoutId); 
        timeoutId = setTimeout(() => {
            applyFiltersAndSort(); 
        }, 750);
    };

    // Los inputs de texto y números usan el Debounce
    [searchInput, minPriceInput, maxPriceInput].forEach(elem => {
        if (elem) elem.addEventListener('input', debouncedApplyFilters);
    });

    // Los selects NO necesitan debounce porque es un solo clic
    [categoryFilter, statusFilter, sortBySelect].forEach(elem => {
        if (elem) elem.addEventListener('change', applyFiltersAndSort);
    });

    // Inicialización general
    initBidManager();
    initAuctionCreator();
    applyFiltersAndSort();
});