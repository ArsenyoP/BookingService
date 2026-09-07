// Simple interactivity for frontend

document.addEventListener('DOMContentLoaded', function() {
    // Star rating for review form
    const stars = document.querySelectorAll('.star');
    const ratingInput = document.getElementById('ratingValue');

    if (stars.length > 0 && ratingInput) {
        stars.forEach(star => {
            star.addEventListener('click', function() {
                const value = this.getAttribute('data-value');
                ratingInput.value = value;

                // Update visual state
                stars.forEach(s => {
                    s.classList.toggle('active', s.getAttribute('data-value') <= value);
                });
            });

            // Hover effect
            star.addEventListener('mouseover', function() {
                const value = this.getAttribute('data-value');
                stars.forEach(s => {
                    s.classList.toggle('hover', s.getAttribute('data-value') <= value);
                });
            });

            star.addEventListener('mouseout', function() {
                stars.forEach(s => {
                    s.classList.remove('hover');
                });
            });
        });
    }

    // Simple mobile menu toggle (if we had a hamburger)
    // const hamburger = document.querySelector('.hamburger');
    // const navMenu = document.querySelector('nav ul');
    // if (hamburger && navMenu) {
    //     hamburger.addEventListener('click', () => {
    //         navMenu.classList.toggle('active');
    //     });
    // }

    // Form submissions (basic prevention for demo)
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            // In a real app, you would send data to API here
            // For demo, just prevent actual submission and show alert
            e.preventDefault();
            alert('Form submitted! (This is a demo - data would be sent to API)');
            // Optionally reset form
            // form.reset();
        });
    });

    // Thumbnail clicks for listing detail
    const thumbnails = document.querySelectorAll('.thumbnails img');
    const mainImg = document.querySelector('.listing-gallery img');

    if (thumbnails.length > 0 && mainImg) {
        thumbnails.forEach(thumb => {
            thumb.addEventListener('click', function() {
                // Update main image
                mainImg.src = this.src;
                // Update active thumbnail
                thumbnails.forEach(t => t.classList.remove('active'));
                this.classList.add('active');
            });
        });
    }

    console.log('BookingService frontend loaded');
});