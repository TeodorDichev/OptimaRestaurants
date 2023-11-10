import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-star-rating',
  templateUrl: './star-rating.component.html',
  styleUrls: ['./star-rating.component.css']
})
export class StarRatingComponent {
  @Input()
  rating!: number;

  get stars(): string[] {
    const fullStars = Math.floor(this.rating);
    const hasHalfStar = this.rating - fullStars >= 0.5;
    const starArray = Array(5).fill('fa-star-o');

    for (let i = 0; i < fullStars; i++) {
      starArray[i] = 'fa-star';
    }
    if (hasHalfStar) {
      starArray[fullStars] = 'fa-star-half-o';
    }
    
    return starArray;
  }
}
