// Rola a vitrine da Loja do Coelho um card por vez.
function scrollLoja(direcao) {
    const track = document.getElementById('lojaTrack');
    if (!track) {
        return;
    }

    const card = track.querySelector('.coelho-loja-card');
    if (card) {
        // Pega a largura do card + o gap (1rem = 16px)
        const cardWidth = card.offsetWidth + 16;
        track.scrollBy({ left: direcao * cardWidth, behavior: 'smooth' });
    }
}