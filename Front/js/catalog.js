import { getFilteredAuctions } from './auctionsService.js';
import { formatPrice, getCurrentUserId } from './utils.js';
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
    const subscribedAuctions = new Set();

    const renderCards = (auctions) => {
        auctionGrid.innerHTML = '';
        if (auctions.length === 0) {
            auctionGrid.innerHTML = '<p style="color:var(--text-muted); grid-column: 1/-1; text-align:center;">No se encontraron subastas.</p>';
            return;
        }
        
        const currentUserId = getCurrentUserId();

        auctions.forEach(subasta => {
            const stateDictionary = {
                'Active': { text: 'En Curso', cssClass: 'activa' },
                'Pending': { text: 'Próximamente', cssClass: 'proxima' },
                'Closed': { text: 'Finalizada', cssClass: 'finalizada' },
                'FinishedWithNoWinner': { text: 'Desierta', cssClass: 'finalizada' }
            };

            const mappedState = stateDictionary[subasta.state] || { text: subasta.state, cssClass: 'finalizada' };
            const isInactive = subasta.state !== 'Active';
            const btnText = subasta.state === 'Pending' ? 'Próximamente' : (isInactive ? 'Subasta Finalizada' : 'Pujar Ahora');

            // ---------------------------------------
            let userStatusHTML = '';

            if (currentUserId && subasta.bids && subasta.bids.length > 0) {
                // Buscamos la puja más alta
                const highestBid = subasta.bids.reduce((max, bid) => bid.amount > max.amount ? bid : max, subasta.bids[0]);
                // Verificamos si el usuario actual participó en esta subasta
                const userHasBids = subasta.bids.some(b => b.buyerId === currentUserId);

                if (highestBid.buyerId === currentUserId) {
                    const msg = subasta.state === 'Closed' ? 'Ganaste esta subasta!' : 'Vas ganando la subasta!';
                    userStatusHTML = `<div class="user-bid-status winning">${msg}</div>`;
                } else if (userHasBids) {
                    const msg = subasta.state === 'Closed' ? 'Subasta finalizada. Perdiste.' : 'Superaron tu puja, ¡volvé a pujar!';
                    userStatusHTML = `<div class="user-bid-status outbid">${msg}</div>`;
                }
            }
            // ----------------------------------------

            const cardHTML = `
            <article class="auction-card" data-id="${subasta.id}" data-state="${subasta.state}" data-price="${subasta.currentPrice}" data-end-date="${subasta.endDate}">
                <div class="card-image-wrapper">
                    <img src="${subasta.urlImage}" alt="${subasta.title}">
                    <span class="badge badge-${mappedState.cssClass}">${mappedState.text}</span>  
                </div>
                <div class="card-body">
                    <h3 class="card-title">${subasta.title}</h3>
                    <p class="card-location">${subasta.description.substring(0, 50)}...</p>
                    
                    ${userStatusHTML}

                    <div class="card-time-info" style="font-size:0.85rem; margin:0.5rem 0; font-weight:600;">
                        <span class="time-countdown">Calculando...</span>
                    </div>
                    <div class="card-price-info">
                        <span class="price-label">Precio Actual:</span>
                        <span class="price-amount">${formatPrice(subasta.currentPrice)}</span>
                    </div>
                    <button class="btn-submit" ${isInactive ? 'disabled' : ''}>${btnText}</button>
                </div>
            </article>
            `;
            auctionGrid.innerHTML += cardHTML;
        });
        if (typeof connection !== 'undefined' && connection.state === signalR.HubConnectionState.Connected) {
            auctions.forEach(subasta => {
                if (!subscribedAuctions.has(subasta.id)) {
                    connection.invoke("JoinAuctionGroup", subasta.id.toString())
                        .catch(err => console.error("Error al unirse al grupo:", err));

                    subscribedAuctions.add(subasta.id);
                }
            });
        }
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
    initBidManager(() => {
        applyFiltersAndSort();
    });

    initAuctionCreator(() => {
        applyFiltersAndSort();
    });

    applyFiltersAndSort();

    
    // SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7281/auctionHub") 
        .withAutomaticReconnect()
        .build();


    connection.on("ReceiveNewBid", (auctionId, newAmount) => {
        console.log(`Puja en tiempo real detectada! Subasta: ${auctionId}, Nuevo Monto: $${newAmount}`);
        applyFiltersAndSort();
    });

    connection.on("ReceiveAuctionClosed", (auctionId, winnerId) => {
        console.log(`Subasta ${auctionId} finalizada.`);
        applyFiltersAndSort();
    });
    
    async function startSignalR() {
        try {
            await connection.start();
            console.log("Conectado exitosamente a SignalR!");
            applyFiltersAndSort();
        } catch (err) {
            console.error("Error al conectar con SignalR:", err);
            setTimeout(startSignalR, 5000); // Reintento en 5 segundos si falla
        }
    }

    startSignalR();
});