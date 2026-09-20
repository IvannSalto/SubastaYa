document.addEventListener('DOMContentLoaded', () => {
    const API_BASE_URL = 'https://localhost:7281/api';

    // Función auxiliar para obtener la sesión actual
    const getSession = () => {
        const session = localStorage.getItem('currentUser');
        return session ? JSON.parse(session) : null;
    };

    const token = localStorage.getItem('token');
    const currentUser = getSession();

    // --- 1. GESTIÓN DE PESTAÑAS (TABS) ---
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    const switchTab = (targetTabId) => {
        tabBtns.forEach(b => b.classList.remove('active'));
        tabContents.forEach(c => {
            c.classList.remove('active');
            c.classList.add('hidden');
        });

        const activeBtn = document.querySelector(`.tab-btn[data-target="${targetTabId}"]`);
        const activeContent = document.getElementById(targetTabId);

        if (activeBtn) activeBtn.classList.add('active');
        if (activeContent) {
            activeContent.classList.remove('hidden');
            activeContent.classList.add('active');
        }

        // Cargar datos según la pestaña activa
        if (targetTabId === 'billetera') {
            loadWalletData();
        } else if (targetTabId === 'pujas') {
            loadMyBids();
        }
    };

    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const target = btn.getAttribute('data-target');
            switchTab(target);
        });
    });

   
    const urlParams = new URLSearchParams(window.location.search);
    const tabParam = urlParams.get('tab');
    if (tabParam) {
        switchTab(tabParam);
    } else {
       
        loadWalletData();
    }


    async function loadWalletData() {
        if (!token || !currentUser || !currentUser.id) return;

        try {
            const response = await fetch(`${API_BASE_URL}/Wallet/user/${currentUser.id}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const apiResponse = await response.json();
                const wallet = apiResponse.data || apiResponse;

                const balance = wallet.availableBalance ?? wallet.AvailableBalance ?? wallet.balance ?? 0;
                const retained = wallet.balanceHeld ?? wallet.BalanceHeld ?? wallet.retainedBalance ?? wallet.RetainedBalance ?? 0;

               
                const dashBalance = document.getElementById('dashAvailableBalance');
                const dashRetained = document.getElementById('dashRetainedBalance');

                if (dashBalance) dashBalance.textContent = `$ ${Number(balance).toLocaleString('es-AR', { minimumFractionDigits: 2 })}`;
                if (dashRetained) dashRetained.textContent = `$ ${Number(dashRetained).toLocaleString('es-AR', { minimumFractionDigits: 2 })}`;

                
                loadWalletMovements();
            }
        } catch (error) {
            console.error('Error al cargar la billetera en el dashboard:', error);
        }
    }

    async function loadWalletMovements() {
        const historyTableBody = document.getElementById('historyTableBody');
        if (!historyTableBody || !currentUser) return;

        try {
            const response = await fetch(`${API_BASE_URL}/Wallet/user/${currentUser.id}/movements`, {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const apiResponse = await response.json();
                const movements = apiResponse.data || apiResponse;

                historyTableBody.innerHTML = '';
                if (!movements || movements.length === 0) {
                    historyTableBody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: var(--text-muted);">No hay movimientos registrados.</td></tr>`;
                    return;
                }

                movements.forEach(mov => {
                    const dateFormatted = new Date(mov.date || mov.createdAt).toLocaleDateString('es-AR', {
                        day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
                    });
                    const isPositive = mov.amount > 0;
                    const amountClass = isPositive ? 'text-success' : 'text-danger';
                    const sign = isPositive ? '+' : '';

                    historyTableBody.innerHTML += `
                        <tr>
                            <td>${dateFormatted}</td>
                            <td>${mov.description || mov.type || 'Movimiento'}</td>
                            <td class="${amountClass}">${sign}$ ${Number(mov.amount).toLocaleString('es-AR', { minimumFractionDigits: 2 })}</td>
                            <td>Completado</td>
                        </tr>
                    `;
                });
            }
        } catch (error) {
            console.error('Error al cargar movimientos:', error);
            historyTableBody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: var(--text-muted);">Error al cargar historial.</td></tr>`;
        }
    }



    async function loadMyBids() {
        const grid = document.getElementById('myBidsGrid');
        if (!grid || !currentUser) return;

        grid.innerHTML = '<p style="color: var(--text-muted); grid-column: 1/-1; text-align: center;">Cargando tus pujas...</p>';

        try {
           
            const response = await fetch(`${API_BASE_URL}/Auction/filter`);
            if (!response.ok) throw new Error('No se pudieron obtener las subastas');

            const data = await response.json();
            const auctions = data.data || data;

            // Filtramos subastas que contengan al menos una puja del usuario actual
            const myAuctions = auctions.filter(subasta => 
                subasta.bids && subasta.bids.some(b => b.buyerId === currentUser.id)
            );

            if (myAuctions.length === 0) {
                grid.innerHTML = '<p style="color: var(--text-muted); grid-column: 1/-1; text-align: center;">Aún no has participado en ninguna subasta.</p>';
                return;
            }

            grid.innerHTML = '';

            myAuctions.forEach(subasta => {
                const userBids = subasta.bids.filter(b => b.buyerId === currentUser.id);
                const myHighestBid = Math.max(...userBids.map(b => b.amount));

                const highestOverallBid = subasta.bids.reduce((max, b) => b.amount > max.amount ? b : max, subasta.bids[0]);
                const isWinning = highestOverallBid.buyerId === currentUser.id;

                const statusText = isWinning ? 'Vas ganando' : 'Superaron tu puja';

                const cardHTML = `
                    <article class="auction-card" style="background: var(--card-bg, #1e1e1e); border: 1px solid var(--border-color, #2a2a2a); border-radius: 8px; overflow: hidden; padding: 1rem;">
                        <div class="card-image-wrapper" style="position: relative; height: 140px; margin-bottom: 1rem;">
                            <img src="${subasta.urlImage}" alt="${subasta.title}" style="width: 100%; height: 100%; object-fit: cover; border-radius: 4px;">
                            <span class="badge" style="position: absolute; top: 8px; right: 8px; padding: 4px 8px; font-size: 0.75rem; border-radius: 4px; background: ${isWinning ? '#10b981' : '#ef4444'}; color: white;">
                                ${statusText}
                            </span>
                        </div>
                        <div class="card-body">
                            <h3 style="font-size: 1rem; margin-bottom: 0.5rem; color: var(--text-main, #fff);">${subasta.title}</h3>
                            <p style="font-size: 0.85rem; color: var(--text-muted, #aaa); margin-bottom: 0.25rem;">Tu puja máxima: <strong>$ ${myHighestBid.toLocaleString('es-AR')}</strong></p>
                            <p style="font-size: 0.85rem; color: var(--text-muted, #aaa); margin-bottom: 1rem;">Precio actual en mesa: <strong>$ ${subasta.currentPrice.toLocaleString('es-AR')}</strong></p>
                            <a href="index.html" class="btn-primary" style="display: block; text-align: center; text-decoration: none; padding: 0.5rem; font-size: 0.85rem; border-radius: 4px;">Ir a la Subasta</a>
                        </div>
                    </article>
                `;
                grid.innerHTML += cardHTML;
            });

        } catch (error) {
            console.error('Error al cargar mis pujas:', error);
            grid.innerHTML = '<p style="color: #ef4444; grid-column: 1/-1; text-align: center;">Error al cargar tus pujas.</p>';
        }
    }

});