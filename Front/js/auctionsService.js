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

function getAuthHeaders() { 
    
    const token = localStorage.getItem('token'); 
    
    const headers = {
        'Content-Type': 'application/json'
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
}

export async function createAuction(auctionData) {
    try {
        let token = localStorage.getItem('token');

        // Si el token se guardó mal como "[object Object]", intentamos rescatarlo de "currentUser"
        if (!token || token === '[object Object]') {
            const currentUserStr = localStorage.getItem('currentUser');
            if (currentUserStr) {
                try {
                    const userObj = JSON.parse(currentUserStr);
                    token = userObj.token || userObj.accessToken || userObj.jwt;
                } catch (e) {}
            }
        }

        if (!token || token === '[object Object]') {
            throw new Error("No hay un token de sesión válido. Por favor, cierra sesión y vuelve a ingresar.");
        }

        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(auctionData)
        });

        if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw new Error(errData.message || `Error al crear subasta: HTTP ${response.status}`);
        }

        const result = await response.json();
        return result.data || result.Data || result;

    } catch (error) {
        console.error("Error al crear subasta:", error);
        throw error;
    }
}
/**
 * Realiza una puja (Corresponde a [HttpPost("{auctionId}/bid")] en tu Controller)
 */
export async function placeBid(auctionId, amount) {
    try {
        const response = await fetch(`${API_BASE_URL}/${auctionId}/bid`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ amount }) // Coincide con tu BidRequest { amount }
        });

        if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw new Error(errData.message || `Error al realizar la puja: HTTP ${response.status}`);
        }

        const result = await response.json();
        return result.data || result.Data || result;

    } catch (error) {
        console.error("Error al pujar:", error);
        throw error;
    }
}

/**cierra una subasta **/
export async function closeAuction(auctionId) {
    try {
        const response = await fetch(`${API_BASE_URL}/${auctionId}/close`, {
            method: 'POST',
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw new Error(errData.message || `Error al cerrar la subasta: HTTP ${response.status}`);
        }

        const result = await response.json();
        return result.data || result.Data || result;

    } catch (error) {
        console.error("Error al cerrar subasta:", error);
        throw error;
    }
}