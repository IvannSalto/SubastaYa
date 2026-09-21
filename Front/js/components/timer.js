let countdownInterval;

export const startCountdown = () => {
    if (countdownInterval) clearInterval(countdownInterval);

    countdownInterval = setInterval(() => {
        const cards = document.querySelectorAll('.auction-card');

        cards.forEach(card => {
            const state = card.getAttribute('data-state');
            const timeElement = card.querySelector('.time-countdown');
            const startDateStr = card.getAttribute('data-start-date'); 
            const endDateStr = card.getAttribute('data-end-date');

            if (!timeElement) return;

            if (state === 'Closed' || state === 'FinishedWithNoWinner') {
                timeElement.textContent = "Finalizada";
                timeElement.style.color = "var(--text-muted)";
                return;
            }

            const isPending = state === 'Pending';
            const targetDateStr = isPending ? startDateStr : endDateStr;

            if (!targetDateStr) return;

            // huso horario arg
            let cleanDateStr = targetDateStr.replace('Z', '');
            if (!cleanDateStr.includes('-03:00')) cleanDateStr += '-03:00';

            const distance = new Date(cleanDateStr).getTime() - new Date().getTime();

            // que pasa si el tiempo llega a cero
            if (distance < 0) {
                if (isPending) {
                    timeElement.textContent = "Iniciando...";
                    timeElement.style.color = "#fbbf24";
                } else {
                    timeElement.textContent = "Tiempo agotado!";
                    timeElement.style.color = "#f87171";
                }
                return;
            }

            // Calculamos horas, minutos y segundos
            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            let timeText = "";
            if (days > 0) timeText += `${days}d `;
            timeText += `${hours}h ${minutes}m ${seconds}s`;

            // diseño visual 
            if (isPending) {
                timeElement.textContent = `Inicia en: ${timeText}`;
                timeElement.style.color = "#fbbf24";
            } else {
                timeElement.textContent = timeText;
                timeElement.style.color = distance < 300000 ? "#f87171" : "var(--accent-color)";
            }
        });
    }, 1000);
};