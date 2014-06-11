/** 
Multilevel
---------------------------------------------------------------------
Author: 		Lane Olson
Version:		1.0
Date: 			April 27, 2012
Description:	Displays multiple lists of links as a dropdown menu
---------------------------------------------------------------------
**/
;(function ($) {

    // initialize namespace if it doesn't exist
    if (!$.responsive) {
            $.responsive = {};
	};
    
	$.responsive.multilevel = function ( el, options ) {
		// To avoid scope issues, use 'base' instead of 'this'
		// to reference this class from internal events and functions.
		var base = this;

		// Access to jQuery and DOM versions of element
		base.$el = $(el);
		base.el = el;

		// Add a reverse reference to the DOM object
		base.$el.data( "responsive.multilevel" , base );
		
		
		var topLevelLi = $('#menu > li', base.el);
        var subLevelLi = $('.submenu > li', base.el);
        var subsubLevelLi = $('.subsubmenu > li', base.el);
        

		base.init = function () {
        			base.options = $.extend({}, $.responsive.multilevel.defaultOptions, options);
			
			base.$el.bind("createmultilevel", function() {
				base.create();
			});
			
			base.$el.bind("destroymultilevel", function() {
				base.destroy();
			});
			
			base.create();
		};
		
		// item - list item containing the sub menu
		base.showDropdown = function(item) {
  
			item.children().show();
		};
		
		/*base.hideDropdown = function() {
			if($('ul > li > ul', base.el).length >= 1) {
				$(this, base.el).hide();
			} else {
				$('ul li ul', base.el).hide();
			}

		};*/
		
		base.create = function()
		{
			// hide the sub navigation
			//base.hideDropdown();
           
            $('.submenu > li > ul', base.el).hide();

			topLevelLi.bind("click mouseenter", function() {
				base.showDropdown($(this));
                
			});
			
			topLevelLi.bind("mouseleave", function () {
				//base.hideDropdown();
                 $('#menu li ul').hide();
                 $('.submenu', base.el).hide();
			});

            subLevelLi.bind("click mouseenter", function() {
				base.showDropdown($(this));
                
			});
			
			subLevelLi.bind("mouseleave", function () {
				//base.hideDropdown();
                 $('.subsubmenu', base.el).hide();
                
			});
		};
		
		base.destroy = function() {
			$('div', base.el).removeAttr('style');
			$('ul', base.el).removeAttr('style');
			topLevelLi.unbind("click mouseenter mouseleave");
		};
		
		base.init();
	};
	
	$.responsive.multilevel.defaultOptions = {
		
	};
	
	$.fn.multilevel = function( options ) {
		return this.each(function () {
				(new $.responsive.multilevel(this, options));
		});
	};
	
})( jQuery );