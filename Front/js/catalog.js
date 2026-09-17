
// modulo 2: filtrado,ordenamiento y pujas

document.addEventListener('DOMContentLoaded', () => {
  // ELEMENTOS DE FILTROS Y BÚSQUEDA
  const searchInput = document.getElementById('searchInput');
  const categoryFilter = document.getElementById('categoryFilter');
  const statusFilter = document.getElementById('statusFilter');
  const minPriceInput = document.getElementById('minPrice');
  const maxPriceInput = document.getElementById('maxPrice');
  const sortBySelect = document.getElementById('sortBy');
  const auctionGrid = document.getElementById('auction-grid');

  // ELEMENTOS DEL MODAL DE PUJA
  const bidModal = document.getElementById('bidModal');
  const closeBidBtn = document.getElementById('closeBidBtn');
  const bidForm = document.getElementById('bidForm');
  const bidItemTitle = document.getElementById('bidItemTitle');
  const bidCurrentPrice = document.getElementById('bidCurrentPrice');
  const bidAmountInput = document.getElementById('bidAmount');
  const bidMinHint = document.getElementById('bidMinHint');

  let activeCardElement = null; // Guarda la tarjeta sobre la cual se está pujando
  let currentActivePrice = 0;

  // --- HELPERS DE MONEDA ---
  const parsePrice = (priceStr) => {
    return parseInt(priceStr.replace(/[^0-9]/g, ''), 10) || 0;
  };

  const formatPrice = (amount) => {
    return '$ ' + amount.toLocaleString('es-AR');
  };

  // --- FUNCIÓN DE NOTIFICACIÓN LOCAL (O reutiliza la global si existe) ---
  const showCatalogToast = (message, isError = false) => {
    let toast = document.getElementById('toast');
    if (!toast) {
      toast = document.createElement('div');
      toast.id = 'toast';
      toast.className = 'toast-notification';
      document.body.appendChild(toast);
    }
    toast.textContent = message;
    if (isError) {
      toast.classList.add('error');
    } else {
      toast.classList.remove('error');
    }
    toast.classList.add('show');
    setTimeout(() => {
      toast.classList.remove('show');
    }, 3500);
  };

  // --- 1. FILTRADO Y ORDENAMIENTO EN TIEMPO REAL ---
  const applyFiltersAndSort = () => {
    if (!auctionGrid) return;
    const cards = Array.from(auctionGrid.querySelectorAll('.auction-card'));
    
    const query = searchInput ? searchInput.value.toLowerCase().trim() : '';
    const selectedCategory = categoryFilter ? categoryFilter.value : 'all';
    const selectedStatus = statusFilter ? statusFilter.value : 'all';
    const minPrice = minPriceInput && minPriceInput.value ? parseFloat(minPriceInput.value) : 0;
    const maxPrice = maxPriceInput && maxPriceInput.value ? parseFloat(maxPriceInput.value) : Infinity;

    // Filtrado dinámico
    const visibleCards = cards.filter(card => {
      const title = card.querySelector('.card-title')?.textContent.toLowerCase() || '';
      const category = card.getAttribute('data-category') || '';
      const state = card.getAttribute('data-state') || '';
      const price = parseFloat(card.getAttribute('data-price')) || 0;

      const matchesSearch = title.includes(query);
      const matchesCategory = selectedCategory === 'all' || category === selectedCategory;
      const matchesStatus = selectedStatus === 'all' || state === selectedStatus;
      const matchesPrice = price >= minPrice && price <= maxPrice;

      const isVisible = matchesSearch && matchesCategory && matchesStatus && matchesPrice;
      card.style.display = isVisible ? 'flex' : 'none';
      
      return isVisible;
    });

    // Ordenamiento de las tarjetas visibles
    const sortCriteria = sortBySelect ? sortBySelect.value : 'default';

    if (sortCriteria !== 'default' && visibleCards.length > 0) {
      visibleCards.sort((a, b) => {
        const priceA = parseFloat(a.getAttribute('data-price')) || 0;
        const priceB = parseFloat(b.getAttribute('data-price')) || 0;
        const timeA = parseInt(a.getAttribute('data-time'), 10) || 0;
        const timeB = parseInt(b.getAttribute('data-time'), 10) || 0;

        if (sortCriteria === 'bid-desc') return priceB - priceA; // Mayor puja
        if (sortCriteria === 'bid-asc') return priceA - priceB;   // Menor puja
        if (sortCriteria === 'time-asc') return timeA - timeB;  // Menor tiempo restante
        return 0;
      });

      // Reordenar físicamente en el DOM
      visibleCards.forEach(card => auctionGrid.appendChild(card));
    }
  };

  // Escuchar eventos en los controles de filtro
  [searchInput, minPriceInput, maxPriceInput].forEach(elem => {
    if (elem) elem.addEventListener('input', applyFiltersAndSort);
  });

  [categoryFilter, statusFilter, sortBySelect].forEach(elem => {
    if (elem) elem.addEventListener('change', applyFiltersAndSort);
  });


  // --- 2. VALIDACIÓN DE ESTADOS Y APERTURA DE MODAL DE PUJA ---
  document.addEventListener('click', (e) => {
    if (e.target && e.target.classList.contains('btn-submit')) {
      e.preventDefault();

      const card = e.target.closest('.auction-card');
      if (!card) return;

      const state = card.getAttribute('data-state');
      const title = card.querySelector('.card-title')?.textContent || 'este lote';

      // Validación de estados (Bloqueo si no está activa)
      if (state === 'proxima') {
        showCatalogToast(`⚠️ La subasta para "${title}" aún no ha comenzado. Próximamente disponible.`);
        return;
      }

      if (state === 'finalizada') {
        showCatalogToast(`❌ La subasta para "${title}" ya ha finalizado. No se aceptan más pujas.`);
        return;
      }

      // VALIDACIÓN DE SESIÓN: Verificar si hay un usuario logueado en localStorage
      const session = localStorage.getItem('currentUser');
      if (!session) {
        showCatalogToast('Debes iniciar sesión para realizar una puja', true);
        const loginModal = document.getElementById('loginModal');
        if (loginModal) loginModal.classList.add('active');
        return;
      }

      // Si está ACTIVA y hay sesión, abrir modal de puja
      activeCardElement = card;

      const priceText = card.querySelector('.price-amount')?.textContent || '$ 0';
      currentActivePrice = parsePrice(priceText);

      const minBid = currentActivePrice + 10000; // Incremento mínimo sugerido

      if (bidItemTitle) bidItemTitle.textContent = title;
      if (bidCurrentPrice) bidCurrentPrice.textContent = formatPrice(currentActivePrice);
      if (bidAmountInput) {
        bidAmountInput.value = minBid;
        bidAmountInput.min = minBid;
      }
      if (bidMinHint) bidMinHint.textContent = `Monto mínimo sugerido: ${formatPrice(minBid)}`;

      if (bidModal) bidModal.classList.add('active');
    }
  });

  const closeBidModalWindow = () => {
    if (bidModal) {
      bidModal.classList.remove('active');
      activeCardElement = null;
      if (bidForm) bidForm.reset();
    }
  };

  if (closeBidBtn) closeBidBtn.addEventListener('click', closeBidModalWindow);

  // Envío de la puja
  if (bidForm) {
    bidForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const enteredBid = parseInt(bidAmountInput.value, 10);

      if (isNaN(enteredBid) || enteredBid <= currentActivePrice) {
        showCatalogToast(`Tu oferta debe ser mayor a ${formatPrice(currentActivePrice)}`, true);
        return;
      }

      // Actualizar visualmente el precio en la tarjeta y el atributo data-price
      if (activeCardElement) {
        const priceElement = activeCardElement.querySelector('.price-amount');
        if (priceElement) {
          priceElement.textContent = formatPrice(enteredBid);
        }
        activeCardElement.setAttribute('data-price', enteredBid);
      }

      showCatalogToast(`¡Puja realizada con éxito por ${formatPrice(enteredBid)}!`);
      closeBidModalWindow();
    });
  }

  // Cerrar modal de puja con clic fuera o tecla ESC
  window.addEventListener('click', (e) => {
    if (e.target === bidModal) closeBidModalWindow();
  });

  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
      if (bidModal && bidModal.classList.contains('active')) closeBidModalWindow();
    }
  });
});


document.addEventListener('DOMContentLoaded', () => {
  const cards = document.querySelectorAll('.auction-card');
  
  cards.forEach(card => {
    const state = card.getAttribute('data-state');
    const btn = card.querySelector('.btn-submit');
    
    if (btn && (state === 'finalizada' || state === 'proxima')) {
      btn.setAttribute('aria-disabled', 'true');
      // Opcional: cambiar el texto del botón según el estado
      if (state === 'finalizada') {
        btn.textContent = 'Subasta Finalizada';
      } else if (state === 'proxima') {
        btn.textContent = 'Próximamente';
      }
    }
  });
});