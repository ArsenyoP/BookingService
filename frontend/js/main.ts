// Simple interactivity for frontend using TypeScript

document.addEventListener('DOMContentLoaded', function() {
    // Star rating for review form
    const stars = document.querySelectorAll<HTMLElement>('.star');
    const ratingInput = document.getElementById('ratingValue') as HTMLInputElement | null;

    if (stars.length > 0 && ratingInput) {
        stars.forEach(star => {
            star.addEventListener('click', function() {
                const value = this.getAttribute('data-value');
                if (value !== null) {
                    ratingInput.value = value;

                    // Update visual state
                    stars.forEach(s => {
                        const sValue = s.getAttribute('data-value');
                        s.classList.toggle('active', sValue !== null && parseInt(sValue) <= parseInt(value));
                    });
                }
            });

            // Hover effect
            star.addEventListener('mouseover', function() {
                const value = this.getAttribute('data-value');
                if (value !== null) {
                    stars.forEach(s => {
                        const sValue = s.getAttribute('data-value');
                        s.classList.toggle('hover', sValue !== null && parseInt(sValue) <= parseInt(value));
                    });
                }
            });

            star.addEventListener('mouseout', function() {
                stars.forEach(s => {
                    s.classList.remove('hover');
                });
            });
        });
    }

    // Form submissions (basic prevention for demo)
    const forms = document.querySelectorAll<HTMLFormElement>('form');
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
    const thumbnails = document.querySelectorAll<HTMLImageElement>('.thumbnails img');
    const mainImg = document.querySelector<HTMLImageElement>('.listing-gallery img');

    if (thumbnails.length > 0 && mainImg) {
        thumbnails.forEach(thumb => {
            thumb.addEventListener('click', function() {
                // Update main image
                if (this.src) {
                    mainImg.src = this.src;
                }
                // Update active thumbnail
                thumbnails.forEach(t => t.classList.remove('active'));
                this.classList.add('active');
            });
        });
    }

    console.log('BookingService frontend loaded with TypeScript');
});