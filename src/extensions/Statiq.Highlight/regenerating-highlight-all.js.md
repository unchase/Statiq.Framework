﻿Highlight-all.js was created by taking the node version of highlight.js and 
using browserify to make it browser ready by applying a workaround for the require statements.

Even though we aren't using it in a browser, this will get it in a state that we can call it from 
the MsIE JS engine. The browser version had a different issue where it expected a browser to be present
when running. 

This gives us one big file with all the languages that doesn't need npm or a browser.

To rebuild

```
npm install browserify
npm install highlight.js
browserify index.js --standalone hljs -o highlight-all.js
```

It then needs a few tweaks to be compliant with the .net regex engine. 

- for ada replace 
  ```
  var BAD_CHARS = '[]{}%#\'\"'
  ```
  with
  ```
  var BAD_CHARS = '{}%#\'\"'
  ```
- for lisp replace
  ```
  var MEC_RE = '\\|[^]*?\\|';
  ```
  with
  ```
  var MEC_RE = '\\|[\S\s]*?\\|';
  ```