(function () {
    // --- hero mini-rotator (unchanged) ---
    const frames = Array.from(document.querySelectorAll('.hero-carousel .hero-frame'));
    const dotsMini = Array.from(document.querySelectorAll('.hero-dots .hero-dot'));
    if (frames.length) {
        let idx = 0;
        const show = (n) => {
            frames.forEach((f, i) => f.classList.toggle('active', i === n));
            dotsMini.forEach((d, i) => d.classList.toggle('active', i === n));
        };
        show(0);
        setInterval(() => { idx = (idx + 1) % frames.length; show(idx); }, 4000);
    }
})();

(function () {
    const stage = document.getElementById('landing-stage');
    const track = document.getElementById('landing-track');
    if (!stage || !track) return;

    const slides = Array.from(track.querySelectorAll('.v-slide'));
    const dots = Array.from(stage.querySelectorAll('.v-dot'));
    if (!slides.length) return;

    const maxIndex = slides.length - 1;
    const clamp = (n, min, max) => Math.max(min, Math.min(max, n));
    const stageHeight = () => stage.getBoundingClientRect().height || window.innerHeight;

    // map data-bg → CSS var, and data-anchor → index
    const anchorMap = new Map();
    slides.forEach((slide, i) => {
        const raw = slide.getAttribute('data-bg');
        if (raw) slide.style.setProperty('--slide-bg', /^url\(/i.test(raw) ? raw : `url('${raw}')`);
        const anchor = (slide.getAttribute('data-anchor') || '').trim().toLowerCase();
        if (anchor) anchorMap.set(anchor, i);
    });

    let index = 0;
    let locked = false;
    const lockMs = 650;

    const setTransform = (animate = true) => {
        const offset = -index * stageHeight();
        if (!animate) {
            const prev = track.style.transition;
            track.style.transition = 'none';
            track.style.transform = `translate3d(0, ${offset}px, 0)`;
            requestAnimationFrame(() => { track.style.transition = prev; });
        } else {
            track.style.transform = `translate3d(0, ${offset}px, 0)`;
        }
    };

    const updateStates = () => {
        dots.forEach((d) => d.setAttribute('aria-current', d.dataset.index === String(index) ? 'true' : 'false'));
        slides.forEach((s, i) => s.classList.toggle('is-active', i === index));
    };

    const goTo = (target, { announce = true, animate = true } = {}) => {
        const next = clamp(target, 0, maxIndex);
        if (next === index) return false;
        index = next;
        setTransform(animate);
        updateStates();
        if (announce) {
            const label = slides[index].getAttribute('aria-label') || `Slide ${index + 1}`;
            track.setAttribute('aria-label', `Showing: ${label}`);
        }
        return true;
    };

    const lockNav = () => { locked = true; setTimeout(() => locked = false, lockMs); };

    // Wheel / keys / touch
    stage.addEventListener('wheel', (e) => {
        const dy = e.deltaY || e.wheelDelta || -e.detail || 0;
        if (locked) return;
        if (dy > 10) { if (goTo(index + 1)) { e.preventDefault(); lockNav(); } }
        if (dy < -10) { if (goTo(index - 1)) { e.preventDefault(); lockNav(); } }
    }, { passive: false });

    window.addEventListener('keydown', (e) => {
        if (locked) return;
        if (e.key === 'ArrowDown' || e.key === 'PageDown') { if (goTo(index + 1)) { e.preventDefault(); lockNav(); } }
        if (e.key === 'ArrowUp' || e.key === 'PageUp') { if (goTo(index - 1)) { e.preventDefault(); lockNav(); } }
        if (e.key === 'Home') { if (goTo(0)) { e.preventDefault(); lockNav(); } }
        if (e.key === 'End') { if (goTo(maxIndex)) { e.preventDefault(); lockNav(); } }
    });

    let touchStartY = null, touchStartX = null;
    const swipeThreshold = 40;
    stage.addEventListener('touchstart', (e) => {
        const t = e.changedTouches[0]; touchStartY = t.clientY; touchStartX = t.clientX;
    }, { passive: true });
    stage.addEventListener('touchend', (e) => {
        if (locked || touchStartY == null) return;
        const t = e.changedTouches[0]; const dy = t.clientY - touchStartY; const dx = t.clientX - touchStartX;
        if (Math.abs(dy) > Math.abs(dx) && Math.abs(dy) > swipeThreshold) {
            if (dy < 0 ? goTo(index + 1) : goTo(index - 1)) lockNav();
        }
        touchStartY = touchStartX = null;
    }, { passive: true });

    dots.forEach((d) => d.addEventListener('click', () => { if (goTo(parseInt(d.dataset.index, 10))) lockNav(); }));

    // In-stage hash links (#how, #features, etc.)
    stage.addEventListener('click', (e) => {
        const link = e.target.closest('a[href^="#"]'); if (!link) return;
        const anchor = (link.getAttribute('href') || '').slice(1).trim().toLowerCase(); if (!anchor) return;
        const target = anchorMap.get(anchor); if (target === undefined) return;
        e.preventDefault();
        if (goTo(target)) {
            lockNav();
            if (history.replaceState) history.replaceState(null, '', `${location.pathname}${location.search}#${anchor}`);
            else location.hash = anchor;
        }
    });

    const applyHash = (animate) => {
        const h = location.hash.replace('#', '').trim().toLowerCase();
        if (!h) return;
        const target = anchorMap.get(h);
        if (target === undefined) return;
        goTo(target, { announce: true, animate });
    };

    // Re-measure when the stage itself resizes (responsive header/footer)
    if (window.ResizeObserver) {
        new ResizeObserver(() => setTransform(false)).observe(stage);
    }
    window.addEventListener('resize', () => setTransform(false));

    // Init
    updateStates();
    setTransform(false);
    const initialLabel = slides[0].getAttribute('aria-label') || 'Slide 1';
    track.setAttribute('aria-label', `Showing: ${initialLabel}`);
    applyHash(false);
})();
