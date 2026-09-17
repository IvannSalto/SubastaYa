const API_BASE_URL = 'https://localhost:7281/api/Auction';

export async function getFilteredAuctions(state, categoryId, sortBy, search, minPrice, maxPrice) {
    try {
        const params = new URLSearchParams();
        if (state && state !== 'all') params.append('state', state);
        if (categoryId && categoryId !== 'all') params.append('categoryId', categoryId);
        if (sortBy && sortBy !== 'default') params.append('sortBy', sortBy);
        if (search) params.append('search', search);
        if (minPrice !== null && minPrice !== undefined && minPrice !== '') {
            params.append('minPrice', minPrice);
        }
        if (maxPrice !== null && maxPrice !== undefined && maxPrice !== '') {
            params.append('maxPrice', maxPrice);
        }

        const response = await fetch(`${API_BASE_URL}/filter?${params.toString()}`);

        if (!response.ok) throw new Error(`Error de red: HTTP ${response.status}`);

        const result = await response.json();
        
        return result.data || result.Data || [];

    } catch (error) {
        console.error("Error conectando a la API:", error);
        return [];
    }
}