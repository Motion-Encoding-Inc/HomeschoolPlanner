(function () {
  const frames = Array.from(document.querySelectorAll('.hero-carousel .hero-frame'));
  const dots = Array.from(document.querySelectorAll('.hero-dots .hero-dot'));
  if (!frames.length) return;

  let index = 0;
  const show = (n) => {
    frames.forEach((frame, idx) => frame.classList.toggle('active', idx === n));
    dots.forEach((dot, idx) => dot.classList.toggle('active', idx === n));
  };

  show(0);
  window.setInterval(() => {
    index = (index + 1) % frames.length;
    show(index);
  }, 4000);
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
  const anchorMap = new Map();

  slides.forEach((slide, idx) => {
    const raw = slide.getAttribute('data-bg');
    if (raw) {
      const value = /^url\(/i.test(raw) ? raw : `url('${raw}')`;
      slide.style.setProperty('--slide-bg', value);
    }
    const anchor = (slide.getAttribute('data-anchor') || '').trim();
    if (anchor) {
      anchorMap.set(anchor.toLowerCase(), idx);
    }
  });

  let index = 0;
  let locked = false;
  const lockMs = 650;

  const setTransform = (animate) => {
    const offset = -index * stageHeight();
    if (animate === false) {
      const previous = track.style.transition;
      track.style.transition = 'none';
      track.style.transform = `translate3d(0, ${offset}px, 0)`;
      window.requestAnimationFrame(() => {
        track.style.transition = previous;
      });
    } else {
      track.style.transform = `translate3d(0, ${offset}px, 0)`;
    }
  };

  const updateStates = () => {
    dots.forEach((dot) => dot.setAttribute('aria-current', dot.dataset.index === String(index) ? 'true' : 'false'));
    slides.forEach((slide, idx) => {
      slide.classList.toggle('is-active', idx === index);
    });
  };

  const goTo = (target, opts = {}) => {
    const { announce = true, animate = true } = opts;
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

  const lockNav = () => {
    locked = true;
    window.setTimeout(() => {
      locked = false;
    }, lockMs);
  };

  stage.addEventListener('wheel', (event) => {
    const delta = event.deltaY || event.wheelDelta || -event.detail || 0;
    if (locked) return;
    if (delta > 10) {
      if (goTo(index + 1)) {
        event.preventDefault();
        lockNav();
      }
    } else if (delta < -10) {
      if (goTo(index - 1)) {
        event.preventDefault();
        lockNav();
      }
    }
  }, { passive: false });

  window.addEventListener('keydown', (event) => {
    if (locked) return;
    if (event.key === 'ArrowDown' || event.key === 'PageDown') {
      if (goTo(index + 1)) {
        event.preventDefault();
        lockNav();
      }
    } else if (event.key === 'ArrowUp' || event.key === 'PageUp') {
      if (goTo(index - 1)) {
        event.preventDefault();
        lockNav();
      }
    } else if (event.key === 'Home') {
      if (goTo(0)) {
        event.preventDefault();
        lockNav();
      }
    } else if (event.key === 'End') {
      if (goTo(maxIndex)) {
        event.preventDefault();
        lockNav();
      }
    }
  });

  let touchStartY = null;
  let touchStartX = null;
  const swipeThreshold = 40;

  stage.addEventListener('touchstart', (event) => {
    const touch = event.changedTouches[0];
    touchStartY = touch.clientY;
    touchStartX = touch.clientX;
  }, { passive: true });

  stage.addEventListener('touchend', (event) => {
    if (locked || touchStartY === null || touchStartX === null) return;
    const touch = event.changedTouches[0];
    const dy = touch.clientY - touchStartY;
    const dx = touch.clientX - touchStartX;
    if (Math.abs(dy) > Math.abs(dx) && Math.abs(dy) > swipeThreshold) {
      if (dy < 0) {
        if (goTo(index + 1)) {
          lockNav();
        }
      } else {
        if (goTo(index - 1)) {
          lockNav();
        }
      }
    }
    touchStartY = touchStartX = null;
  }, { passive: true });

  dots.forEach((dot) => {
    dot.addEventListener('click', () => {
      if (goTo(parseInt(dot.dataset.index, 10))) {
        lockNav();
      }
    });
  });

  stage.addEventListener('click', (event) => {
    const link = event.target.closest('a[href^="#"]');
    if (!link) return;
    const anchor = (link.getAttribute('href') || '').slice(1).trim().toLowerCase();
    if (!anchor) return;
    const target = anchorMap.get(anchor);
    if (target === undefined) return;
    event.preventDefault();
    if (goTo(target)) {
      lockNav();
      if (history.replaceState) {
        history.replaceState(null, '', `${window.location.pathname}${window.location.search}#${anchor}`);
      } else {
        window.location.hash = anchor;
      }
    }
  });

  const applyHash = (animate) => {
    const hash = window.location.hash.replace('#', '').trim().toLowerCase();
    if (!hash) return;
    const target = anchorMap.get(hash);
    if (target === undefined) return;
    goTo(target, { announce: true, animate });
  };

  window.addEventListener('hashchange', () => applyHash(true));

  window.addEventListener('resize', () => setTransform(false));

  updateStates();
  setTransform(false);
  const initialLabel = slides[0].getAttribute('aria-label') || 'Slide 1';
  track.setAttribute('aria-label', `Showing: ${initialLabel}`);
  applyHash(false);
})();
