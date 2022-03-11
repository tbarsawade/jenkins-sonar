(function () {
	/*
	 *  General helper functions
	 */
	var TRUE = !0,
		FALSE = !1,
		NULL = null,
		ATTR_VALUE_TRUE = "true",
		ATTR_VALUE_FALSE = "false",
		ariaAttributeMapping = {
			"disabled": "aria-disabled",
			"expanded": "aria-expanded",
			"hidden": "aria-hidden",
			"pressed": "aria-pressed"
		},
		ua = navigator.userAgent.toLowerCase(),
		IS_MSIE = ua.match(/msie\s/i),
		IS_MSIE7 =  ua.match(/msie\s7/),
		IS_MSIE8 = !!(window.external && typeof (window.external.AddToFavoritesBar) != "undefined"),
		IS_TOUCH = !!("ontouchstart" in document.documentElement),
		findByClass = ( function () {
			if (document.querySelector && typeof document.querySelector == "function") {
				return function (elt, className) {
					var targetNode = elt || document.body;
					return elt.querySelector("." + className);
				};
			}
			return function (elt, className) {
				var regex, elts;
				
				if (!elt)
					elt = document.body;
				
				regex = new RegExp('\\b' + className + '\\b');
				elts = elt.getElementsByTagName("*");
				
				for (var i = 0, j = elts.length; i < j; i++) {
					if (regex.test(elts[i].className))
						return elts[i];
				}
				return null;
			};
		})(),
		// Get the ARIA attribute value on a given DOM eLement
		getAria = function (name, node) {
			var stateName = ariaAttributeMapping[name];
			
			if (!stateName || !node) 
				throw new Error("Passed argument is not valid WIA-ARIA name.");
	
			return node.getAttribute(stateName) || NULL;
		},
		// Check if a given DOM eLement ARIA attribute value on
		hasAria = function (name, node) {
			return (getAria(name, node) != NULL) ? TRUE : FALSE;
		},
		// Boolean function to see if element has particular class 
		hasClass = function (elt/*, className1, className2, classNameN*/) {
			var names = Array.prototype.slice.call(arguments, 1),
				test = " " + elt.className + " ",
				result = names.length,
				i = 0,
				name;
	
			while (result && (name = names[i++])) {
				result = test.indexOf(" " + name + " ") > -1;
			}
	
			return result;
		},
		// Set the value of a specified ARIA attribute value on a given DOM eLement
		setAria = function (name, value, node) {
			var stateName = ariaAttributeMapping[name];
			
			if (!stateName || !node) 
				throw new Error("Passed argument is not valid WIA-ARIA name.");
			
			node.setAttribute(stateName, (value) ? ATTR_VALUE_TRUE : ATTR_VALUE_FALSE);
		};
	
	/*
	 * HTML UI container used for display controls & descriptions.
	 */
	this.NoteContainer = function (args) {
		if (!args || !args.id || !args.parent || !args.title || !args.content)
			throw new Error("Passed arguments to NoteContainer are not valid.");
	
		this.id = args.id;
		this.parent = args.parent;
		
		this.init(args.title, args.content, args.position);
	};
	
	NoteContainer.prototype = {
		init: function (title, content, position) {
			var container = this.container = document.createElement("div"),
				closeButton;
			
			container.setAttribute("id", this.id);
			/* If browser is IE7 old ovi_web UI is used so we have
			 * move all NoteContianer to the right so they won't overlap
			 * Zoombar.
			 */
			if (IS_MSIE7) {
				container.className = "note note_msie7";
			} else {
				container.className = "note note_not_msie7";
			}
			container.innerHTML =
				'<a role="button" class="button close">x</a>' +
				'<div class="title"><h1>' + title + '</h1></div>' +
				'<div class="inner">' + content + '</div>';
				
			// If position is given in the arguments
			if (position) {
				container.style.position = "absolute";
				if (position.top) container.style.top = position.top + "px";
				if (position.right) container.style.right = position.right + "px";
				if (position.bottom) container.style.bottom = position.bottom + "px";
				if (position.left) container.style.left = position.left + "px";
			}
				
			// append new container node to given parent node
			this.parent.appendChild(container);
			
			/* Find the created close button and install onclick handler to 
			 * close the note Container on mouse click.
			 */
			closeButton = findByClass(container, "close");
			closeButton.onclick = function () {
				setAria("hidden", TRUE, container);
				// IE8 has rendering issue with ARIA styling
				if (IS_MSIE8) {
					setTimeout(function () {
						container.style.display = "none";
					}, 100);
				}
			};
		},
		getInner: function () {
			return findByClass(this.container, "inner");
		}
	};
	
	this.Logger = function (args) {
		if (!args || !args.id || !args.parent)
			throw new Error("Passed arguments to NoteContainer are not valid.");
	
		this.id = args.id;
		this.parent = args.parent;
		this.init(args.title || "Message Log", args.content, args.position);
	};
	
	Logger.prototype = {
		init: function (title, content, position) {
			var container = this.container = document.createElement("div"),
				consoleElt,
				clearButton,
				closeButton,
				consoleTitle = this.consoleTitle = '<div class="title">' + title + '</div>';
			
			content = content || consoleTitle;
			container.setAttribute("id", this.id);
			/* If browser is IE7 old ovi_web UI is used so we have
			 * move all NoteContianer to the right so they won't overlap
			 * Zoombar.
			 */
			if (IS_MSIE7) {
				container.className = "log log_msie7";
			} else { 
				container.className = "log log_not_msie78";
			}
			container.innerHTML =
				'<a role="button" class="button close">x</a>' +
				'<div role="button" class="clear button">clear</div>' + 
				'<div class="inner"><div class="consoleElt">' + content + '</div></div>';
			
			// If position is given in the arguments
			if (position) {
				if (position.top) container.style.top = position.top + "px";
				if (position.right) container.style.right = position.right + "px";
				if (position.bottom) container.style.bottom = position.bottom + "px";
				if (position.left) container.style.left = position.left + "px";
			}
			
			// Append new container node to given parent node
			this.parent.appendChild(container);
	
			// Find the created close & clear button and install event handlers
			consoleElt = findByClass(container, "consoleElt");
			clearButton = findByClass(container, "clear");
			clearButton.onclick = function () {
				consoleElt.innerHTML = consoleTitle;
				// Fix for touch screen (iOS etc) not handling CSS onHover properly
				if (IS_TOUCH) {
					clearButton.style.backgroundColor = "#FFF";
					clearButton.style.color = "#999999";
				}
			};
			
			closeButton = findByClass(container, "close");
			closeButton.onclick = function () {
				setAria("hidden", TRUE, container);
			};
		},
		clear: function () {
			var container = this.container,
				consoleElt,
				consoleTitle = this.consoleTitle;
			
			if (container && consoleTitle) {
				consoleElt = findByClass(container, "consoleElt");
				if (consoleElt)
					consoleElt.innerHTML = consoleTitle;
			}
		},
		log: function (msg, append) {
			var newChild,
				consoleElt = findByClass(this.container, "consoleElt"),
				title = findByClass(consoleElt, "title"),
				childNodes,
				l;
			
			// If we are still displaying the title, remove it
			if (title) {
				consoleElt.removeChild(title);
			}
			
			childNodes = consoleElt.childNodes,
			l = childNodes.length;
			
			if (typeof msg === "object" && msg.nodeType === 1 && msg.ownerDocument) {
				newChild = msg;
			} else {
				newChild = document.createElement("p");
				newChild.innerHTML = msg.toString();
				
				// Add class to accomplish odd / even styling
				if (l % 2 != 0) {
					newChild.className = "odd";
				}
			}
			/* Depending on the append argument we either insert a new log message 
			 * before the existing ones (append == true) or after (append == false)
			 */
			l < 1 || append ? consoleElt.appendChild(newChild) : consoleElt.insertBefore(newChild, childNodes[0]);
		}
	};
	
	this.loadScript = function (url, callback) {
		var script = document.createElement("script"),
			head = document.getElementsByTagName("head")[0];
		script.type = "text/javascript";
	
		if (script.readyState) {  // Internet explorer
			script.onreadystatechange = function () {
				if (script.readyState == "loaded" ||
						script.readyState == "complete") {
					script.onreadystatechange = null;
					callback();
				}
			};
		} else { // All other browsers
			script.onload = function () {
				callback();
			};
		}
	
		script.src = url;
		head.appendChild(script);
	};

	this.IS_MSIE = IS_MSIE;
	this.IS_MSIE7 = IS_MSIE7;
	this.IS_MSIE8 = IS_MSIE8;

	if (IS_MSIE && document.documentMode < 7) {
		alert("The Internet Explorer version you're using is not supported. Please update to new one.");
	}

	/* Simple JSONP helper namespace to facilitate cross site JSON delivery for this example
	 * More info on JSONP here: http://en.wikipedia.org/wiki/JSONP
	 * For a more robust implementation use a dedicated library such as jQuery
	 */

	// IE workaround, set netUtil to the window
	this.netUtil = {
		// jsonpRequest constructor
		JSONPRequest : function (url) {
			this.requestUrl = url;
		},
		// jsonpCallback constructor
		JSONPCallback : function (callback) {
			this.callback = callback;
		},
		// a queue to hold the callbacks defined per request
		callbackQueue : []
	};

	this.netUtil.JSONPRequest.prototype.send = function (callback) {
			// Add the specified callback to the queue and get it's index on the array
		var callbackIndex = netUtil.callbackQueue.push(new netUtil.JSONPCallback(callback)) - 1,
			// Specify a unique id for the callback
			callbackId = +new Date(),
			// Define the http GET params for the callback to the remote service
			callbackStr = encodeURIComponent("(function () { netUtil.callbackQueue[" + callbackIndex + "].exec(" + callbackId + ", arguments); })"),
			// Define the script tag that will perform the JSONP call;
			scriptTag = document.createElement("script");

		// Append the callback url
		this.requestUrl = (this.requestUrl.indexOf("jsoncallback=") == -1) ? this.requestUrl + "&jsoncallback=" + callbackStr : this.requestUrl;
		
		// Prepare the script tag
		scriptTag.setAttribute("type", "text/javascript");
		scriptTag.setAttribute("charset", "UTF-8");
		scriptTag.setAttribute("id", "callback_" + callbackId);
		scriptTag.setAttribute("src", this.requestUrl);

		// Perform the JSONP call by adding a script tag with that exact url to the body
		document.body.appendChild(scriptTag);
	};

	this.netUtil.JSONPCallback.prototype.exec = function (callbackId, args) {
		// execute the associated callback if any
		if (this.callback && (typeof this.callback === "function")) {
			if (args.length > 0) {
				this.callback(args[0]);
			} 
		}
		// remove ourselves from the page
		var scriptTag = document.getElementById("callback_" + callbackId);
		if (scriptTag) {
			document.body.removeChild(scriptTag);
		}
	};

})(window);