import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-restaurant-map',
  templateUrl: './restaurant-map.component.html',
  styleUrls: ['./restaurant-map.component.css']
})
export class RestaurantMapComponent implements OnInit {
  @Input() longitude!: string;
  @Input() latitude!: string;

  url: string = '';
  safeUrl: SafeResourceUrl = '';

  constructor(private sanitizer: DomSanitizer) {}

  ngOnInit() {
    this.url = ``;
    this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.url);
  }
}
