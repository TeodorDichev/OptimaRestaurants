import { Component } from '@angular/core';

@Component({
  selector: 'app-rate-employee-restaurant',
  templateUrl: './rate-employee-restaurant.component.html',
  styleUrls: ['./rate-employee-restaurant.component.css']
})
export class RateEmployeeRestaurantComponent {

  stars: Array<{ filled: boolean; color: string; }> = [];
  selectedStar: number | undefined;

  ngOnInit(): void {
    this.initializeStars();
  }

  initializeStars(): void {
    for (let i = 0; i < 5; i++) {
      this.stars.push({ filled: false, color: 'goldenrod' });
    }
  }

  onStarHover(index: number): void {
    for (let i = 0; i <= index; i++) {
      this.stars[i].filled = true;
    }
  }

  onStarLeave(index: number): void {
    if (!this.selectedStar) {
      this.clearFill();
    }
    else if (this.selectedStar <= index) {
      for (let i = 4; i > this.selectedStar - 1; i--) {
        this.stars[i].filled = false;
      }
    }
  }

  onStarClick(index: number): void {
    this.clearFill();
    this.selectedStar = index + 1;
    for (let i = 0; i <= index; i++) {
      this.stars[i].filled = true;
    }
  }

  private clearFill() {
    this.stars.forEach(star => (star.filled = false));
  }
}
