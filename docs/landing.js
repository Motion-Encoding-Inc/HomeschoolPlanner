// Simple auto-rotating carousel: cycles screenshots every 4s
const slides=[...document.querySelectorAll('.slide')];
const dots  =[...document.querySelectorAll('.dot')];
let i=0; function show(n){slides.forEach((s,k)=>s.classList.toggle('active',k===n));dots.forEach((d,k)=>d.classList.toggle('active',k===n));}
if(slides.length){ show(0); setInterval(()=>{ i=(i+1)%slides.length; show(i); }, 4000); }

// ---- Theme toggle with persistence ----
(function(){
  const key = 'hs_theme';
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  const current = localStorage.getItem(key) || (prefersDark ? 'dark' : 'light');
  document.documentElement.setAttribute('data-theme', current);
  const btn = document.getElementById('theme-toggle');
  if(btn){
    btn.addEventListener('click', ()=>{
      const next = (document.documentElement.getAttribute('data-theme') === 'dark') ? 'light' : 'dark';
      document.documentElement.setAttribute('data-theme', next);
      localStorage.setItem(key, next);
    });
  }
})();
