
// modulo 1: filtrado y ordenamienot


document.addEventListener('DOMContentLoaded', () => {
  const searchInput = document.getElementById('searchInput');
  const categoryFilter = document.getElementById('categoryFilter');
  const statusFilter = document.getElementById('statusFilter');
  const minPriceInput = document.getElementById('minPrice');
  const maxPriceInput = document.getElementById('maxPrice');
  const sortBySelect = document.getElementById('sortBy');
  const auctionGrid = document.getElementById('auction-grid');

  if (!auctionGrid) return; // Si no estamos en la página del catálogo, no hace nada

  const applyFiltersAndSort = () => {
    const cards = Array.from(auctionGrid.querySelectorAll('.auction-card'));
    
    const query = searchInput ? searchInput.value.toLowerCase().trim() : '';
    const selectedCategory = categoryFilter ? categoryFilter.value : 'all';
    const selectedStatus = statusFilter ? statusFilter.value : 'all';
    const minPrice = minPriceInput && minPriceInput.value ? parseFloat(minPriceInput.value) : 0;
    const maxPrice = maxPriceInput && maxPriceInput.value ? parseFloat(maxPriceInput.value) : Infinity;

    // 1. Filtrado dinámico
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
      
      // Mostrar u ocultar la tarjeta visualmente
      card.style.display = isVisible ? 'flex' : 'none';
      
      return isVisible;
    });

    // 2. Ordenamiento de las tarjetas visibles
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

  // Escuchar eventos en tiempo real en todos los controles
  [searchInput, minPriceInput, maxPriceInput].forEach(elem => {
    if (elem) elem.addEventListener('input', applyFiltersAndSort);
  });

  [categoryFilter, statusFilter, sortBySelect].forEach(elem => {
    if (elem) elem.addEventListener('change', applyFiltersAndSort);
  });
});