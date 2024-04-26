import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-star-rating',
  templateUrl: './star-rating.component.html',
  styleUrls: ['./star-rating.component.css']
})
export class StarRatingComponent {
  @Input() rating!: number;

  get stars(): string[] {
    const fullStars = Math.floor(this.rating);
    const hasHalfStar = this.rating - fullStars >= 0.5;
    const starArray = Array(5).fill('fa-regular fa-star');

    for (let i = 0; i < fullStars; i++) {
      starArray[i] = 'fa-solid fa-star';
    }
    if (hasHalfStar) {
      starArray[fullStars] = 'fa-regular fa-star-half-stroke';
    }
    
    return starArray;
  }
}
