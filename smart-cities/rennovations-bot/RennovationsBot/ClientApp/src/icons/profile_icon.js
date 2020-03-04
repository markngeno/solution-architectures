import React from "react";

const SvgComponent = props => (
  <svg data-name="Layer 1" viewBox="0 0 20 20" {...props}>
    <path
      d="M10 0a10 10 0 1 0 10 10A10 10 0 0 0 10 0zm0 3a3 3 0 1 1-3 3 3 3 0 0 1 3-3zm0 14.2A7.2 7.2 0 0 1 4 14c0-2 4-3.08 6-3.08S16 12 16 14a7.2 7.2 0 0 1-6 3.2z"
      fill="#fff"
    />
  </svg>
);

export default SvgComponent;
