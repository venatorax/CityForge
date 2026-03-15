declare module '*.scss';
declare module '*.css';
declare module '*.svg'; // we recommed using SVGs for all the icons and UI elements
declare module '*.png';
declare module '*.jpg';
declare module '*.gif';
declare module '*.ttf' { const src: string; export default src; }
declare module '*.woff' { const src: string; export default src; }
declare module '*.woff2' { const src: string; export default src; }