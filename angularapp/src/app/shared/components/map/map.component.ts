import { Component, Input } from '@angular/core';
import * as Leaflet from 'leaflet';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent {
  @Input() hasSearchBar: boolean = true;
  map: any;
  markers: Leaflet.LayerGroup = Leaflet.layerGroup();
  addressSearchResults: any[] = [];
  selectedLocation: any;

  ngOnInit(): void {
    this.initializeMap();
  }

  initializeMap(): void {
    this.map = Leaflet.map('leafletMap').setView([42.605, 23.39], 10);

    Leaflet.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
    }).addTo(this.map);

    this.map.addLayer(this.markers);
    this.map.on('click', (e: Leaflet.LeafletMouseEvent) => this.onMapClick(e));
  }

  onMapClick(e: Leaflet.LeafletMouseEvent): void {
    this.markers.clearLayers();
    const marker = Leaflet.marker(e.latlng);
    this.markers.addLayer(marker);

    /* Latitude: e.latlng.lat.toFixed(6), Longitude: e.latlng.lng.toFixed(6) */
  }

  searchAddress(userInput: any) {
    const apiKey = 'c7e582b61584497ca5c4d41eca86078a';
    const searchString = userInput.target.value;
    const limitResults = 3;

    var url = `https://api.geoapify.com/v1/geocode/autocomplete?text=${searchString}&format=json&limit=${limitResults}&apiKey=${apiKey}`;

    if (searchString.length >= 3) {
      fetch(url, { method: 'GET' })
        .then(response => response.json())
        .then(result => this.addressSearchResults = result.results)
        .catch(error => console.log('error', error));
    }
    else {
      this.addressSearchResults = [];
    }

    console.log(this.addressSearchResults);
  }
}
