"use strict";(self.webpackChunkangularapp=self.webpackChunkangularapp||[]).push([[493],{7493:(w,c,a)=>{a.r(c),a.d(c,{RestaurantsModule:()=>_});var i=a(6814),d=a(6208),p=a(3377),t=a(4769),g=a(1711),m=a(8289),f=a(2174),v=a(9882);function x(n,r){1&n&&(t.TgZ(0,"span",15),t._uU(1,"\u0414\u0430"),t.qZA())}function h(n,r){1&n&&(t.TgZ(0,"span",15),t._uU(1,"\u041d\u0435"),t.qZA())}function A(n,r){if(1&n&&(t.TgZ(0,"div")(1,"div",12),t._uU(2,"\u041d\u0430\u0439-\u0434\u043e\u0431\u044a\u0440 \u0440\u0430\u0431\u043e\u0442\u043d\u0438\u043a: "),t.TgZ(3,"div",16),t._uU(4),t.qZA()(),t.TgZ(5,"div",12),t._uU(6,"\u041d\u0435\u0433\u043e\u0432\u0438\u044f\u0442 \u0440\u0435\u0439\u0442\u0438\u043d\u0433: "),t.TgZ(7,"div"),t._UZ(8,"app-star-rating",11),t.qZA()()()),2&n){const e=t.oxw().$implicit;t.xp6(4),t.Oqu(e.topEmployeeFullName),t.xp6(4),t.Q6J("rating",e.topEmployeeRating)}}function Z(n,r){if(1&n){const e=t.EpF();t.TgZ(0,"div",5)(1,"div",6),t.NdJ("click",function(){const o=t.CHM(e).$implicit,l=t.oxw();return t.KtG(l.getRestaurantDetails(o.id))}),t.TgZ(2,"img",7),t.NdJ("error",function(){const o=t.CHM(e).$implicit,l=t.oxw();return t.KtG(l.missingIcon(o))}),t.qZA(),t.TgZ(3,"div")(4,"div",8),t._uU(5),t.qZA(),t.TgZ(6,"div",9),t._uU(7),t.qZA(),t.TgZ(8,"div",9),t._uU(9),t.qZA()(),t.TgZ(10,"div")(11,"div",10),t._uU(12,"\u0421\u0440\u0435\u0434\u043d\u0430 \u043e\u0446\u0435\u043d\u043a\u0430: "),t._UZ(13,"app-star-rating",11),t.qZA(),t.TgZ(14,"div",12),t._uU(15,"\u0420\u0430\u0431\u043e\u0442\u0435\u0449: "),t.YNc(16,x,2,0,"span",13),t.YNc(17,h,2,0,"span",13),t.qZA()(),t.YNc(18,A,9,2,"div",14),t.qZA()()}if(2&n){const e=r.$implicit;t.xp6(2),t.s9C("src",e.iconPath,t.LSH),t.xp6(3),t.Oqu(e.name),t.xp6(2),t.Oqu(e.address),t.xp6(2),t.Oqu(e.city),t.xp6(4),t.Q6J("rating",e.restaurantAverageRating),t.xp6(3),t.Q6J("ngIf",e.isWorking),t.xp6(1),t.Q6J("ngIf",!e.isWorking),t.xp6(1),t.Q6J("ngIf",e.topEmployeeEmail)}}const R=[{path:"",component:(()=>{class n{constructor(e,s){this.sharedService=e,this.restaurantsService=s,this.allRestaurants=[]}ngOnInit(){this.getAllRestaurants()}getAllRestaurants(){this.restaurantsService.getAllRestaurants(1).subscribe({next:e=>{this.allRestaurants=e,this.currentRestaurant=this.allRestaurants[0]}})}getRestaurantDetails(e){this.sharedService.openRestaurantDetailsModal(e)}selectedRestaurant(e){this.currentRestaurant=e}missingIcon(e){e.iconPath="assets/images/logo-bw-with-bg.png"}static#t=this.\u0275fac=function(s){return new(s||n)(t.Y36(g.F),t.Y36(m.P))};static#e=this.\u0275cmp=t.Xpm({type:n,selectors:[["app-browse-all-restaurants"]],decls:7,vars:1,consts:[[1,"content"],[1,"content-screen","content-screen-fill"],[1,"text-muted","fst-italic","h4"],[1,"rest-list","screen-left"],["class","list-items",4,"ngFor","ngForOf"],[1,"list-items"],[1,"restaurant",3,"click"],[1,"rest-img",3,"src","error"],[1,"h2","fw-bold"],[1,"h6","text-muted"],[1,"h5","fw-bold"],[3,"rating"],[1,"fw-bold"],["class","fw-normal fst-italic",4,"ngIf"],[4,"ngIf"],[1,"fw-normal","fst-italic"],[1,"fw-normal"]],template:function(s,u){1&s&&(t._UZ(0,"nav-top"),t.TgZ(1,"div",0)(2,"div",1)(3,"div",2),t._uU(4," \u0412\u0441\u0438\u0447\u043a\u0438 \u0440\u0435\u0433\u0438\u0441\u0442\u0440\u0438\u0440\u0430\u043d\u0438 \u0440\u0435\u0441\u0442\u043e\u0440\u0430\u043d\u0442\u0438 "),t.qZA(),t.TgZ(5,"div",3),t.YNc(6,Z,19,8,"div",4),t.qZA()()()),2&s&&(t.xp6(6),t.Q6J("ngForOf",u.allRestaurants))},dependencies:[i.sg,i.O5,f.T,v.S],styles:["app-browse-all-restaurants{background-color:#ffe7b9;display:flex;flex-direction:column;align-items:flex-start;flex:1 0 0;align-self:stretch}.rest-img[_ngcontent-%COMP%]{max-width:300px;aspect-ratio:3/2;width:100%!important;object-fit:cover;border-radius:8px 0 0 8px}.empl-img[_ngcontent-%COMP%]{max-width:150px;aspect-ratio:1/1;width:100%!important;object-fit:cover;border-radius:8px}.rest-list[_ngcontent-%COMP%]{gap:12px;overflow:scroll}.list-items[_ngcontent-%COMP%]{display:flex;align-self:stretch}.restaurant[_ngcontent-%COMP%]{display:flex;flex-direction:row;align-self:stretch;flex:1 0 0;gap:20px;border-radius:8px;justify-content:space-between;align-items:center;border:1px solid rgba(128,128,128,.336);padding-right:20px}.restaurant[_ngcontent-%COMP%]:hover{background-color:#f1f1f1cc;cursor:pointer}.content-screen[_ngcontent-%COMP%]{justify-content:flex-start;padding-right:0}.content[_ngcontent-%COMP%]{padding-bottom:0;padding-left:0}"]})}return n})()}];let C=(()=>{class n{static#t=this.\u0275fac=function(s){return new(s||n)};static#e=this.\u0275mod=t.oAB({type:n});static#n=this.\u0275inj=t.cJS({imports:[p.Bz.forChild(R),p.Bz]})}return n})(),_=(()=>{class n{static#t=this.\u0275fac=function(s){return new(s||n)};static#e=this.\u0275mod=t.oAB({type:n});static#n=this.\u0275inj=t.cJS({imports:[i.ez,C,d.m]})}return n})()}}]);