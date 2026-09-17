let countdownInterval;

export const startCountdown = () => {
    if (countdownInterval) clearInterval(countdownInterval);

    countdownInterval = setInterval(() => {
        const cards = document.querySelectorAll('.auction-card');

        cards.forEach(card => {
            const state = card.getAttribute('data-state');
            const timeElement = card.querySelector('.time-countdown');
            const endDateStr = card.getAttribute('data-end-date');

            if (!timeElement || !endDateStr) return;

            if (state !== 'Active' && state !== 'active') {
                timeElement.textContent = state === 'Pending' ? "Aún no inicia" : "Finalizada";
                timeElement.style.color = "var(--text-muted)";
                return;
            }

            let cleanDateStr = endDateStr.replace('Z', '');
            if (!cleanDateStr.includes('-03:00')) cleanDateStr += '-03:00';

            const distance = new Date(cleanDateStr).getTime() - new Date().getTime();

            if (distance < 0) {
                timeElement.textContent = "Tiempo agotado!";
                timeElement.style.color = "#f87171";
                return;
            }

            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            let timeText = "";
            if (days > 0) timeText += `${days}d `;
            timeElement.textContent = timeText + `${hours}h ${minutes}m ${seconds}s`;
            timeElement.style.color = distance < 300000 ? "#f87171" : "var(--accent-color)";
        });
    }, 1000);
};