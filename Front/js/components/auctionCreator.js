import { createAuction } from '../auctionsService.js';
import { showToast } from '../utils.js';

export function initAuctionCreator(onAuctionCreated) {
    const createModal = document.getElementById('createAuctionModal');
    const openModalBtn = document.getElementById('openCreateModalBtn');
    const closeModalBtn = document.getElementById('closeCreateModalBtn');
    const createForm = document.getElementById('createAuctionForm');

    // Validación y apertura del modal al hacer clic en "Crear Subasta"
    if (openModalBtn && createModal) {
        openModalBtn.addEventListener('click', () => {
            const session = localStorage.getItem('currentUser');
            if (!session) {
                showToast('Debes iniciar sesión para crear una subasta', true);
                document.getElementById('loginModal')?.classList.add('active');
                return;
            }
            createModal.classList.add('active');
        });
    }

    const closeModal = () => {
        if (createModal) {
            createModal.classList.remove('active');
            if (createForm) createForm.reset();
        }
    };

    if (closeModalBtn) closeModalBtn.addEventListener('click', closeModal);

    if (createForm) {
        createForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const title = document.getElementById('auctionTitle').value.trim();
            const description = document.getElementById('auctionDescription').value.trim();
            const basePrice = parseFloat(document.getElementById('auctionBasePrice').value);
            const endDateInput = document.getElementById('auctionEndDate').value;
            const categoryId = parseInt(document.getElementById('auctionCategory').value, 10);
            
            const imageInput = document.getElementById('auctionImageFile');
            const file = imageInput ? imageInput.files[0] : null;

            if (!title || !description || isNaN(basePrice) || !endDateInput || isNaN(categoryId) || !file) {
                showToast('Por favor, completa todos los campos y selecciona una imagen.', true);
                return;
            }

            const submitBtn = createForm.querySelector('button[type="submit"]');

            const reader = new FileReader();
            reader.onload = async function (uploadEvent) {
                const urlImage = uploadEvent.target.result;

                const auctionData = {
                    title,
                    description,
                    basePrice,
                    endDate: new Date(endDateInput).toISOString(), 
                    categoryId,
                    urlImage
                };

                try {
                    if (submitBtn) submitBtn.disabled = true;

                    await createAuction(auctionData);

                    showToast('¡Subasta creada exitosamente!');
                    closeModal();

                    if (typeof onAuctionCreated === 'function') {
                        onAuctionCreated();
                    }

                } catch (error) {
                    showToast(error.message || 'No se pudo crear la subasta.', true);
                } finally {
                    if (submitBtn) submitBtn.disabled = false;
                }
            };

            reader.readAsDataURL(file);
        });
    }

    window.addEventListener('click', (e) => { 
        if (e.target === createModal) closeModal(); 
    });
    
    document.addEventListener('keydown', (e) => { 
        if (e.key === 'Escape' && createModal?.classList.contains('active')) closeModal(); 
    });
}