// Simple auto-rotating carousel: cycles screenshots every 4s
const slides=[...document.querySelectorAll('.slide')];
const dots  =[...document.querySelectorAll('.dot')];
let i=0; function show(n){slides.forEach((s,k)=>s.classList.toggle('active',k===n));dots.forEach((d,k)=>d.classList.toggle('active',k===n));}
if(slides.length){ show(0); setInterval(()=>{ i=(i+1)%slides.length; show(i); }, 4000); }
