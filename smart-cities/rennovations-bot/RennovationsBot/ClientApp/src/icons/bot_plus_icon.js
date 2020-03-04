import React from "react";

const SvgBotPlusIcon = props => (
  <svg id="Layer_1" data-name="Layer 1" viewBox="0 0 32 32" {...props}>
    <defs>
      <style>{`.cls-2{fill:#fff}`}</style>
      <linearGradient
        id="linear-gradient"
        x1={4.69}
        y1={4.69}
        x2={27.31}
        y2={27.31}
        gradientUnits="userSpaceOnUse"
      >
        <stop offset={0} stopColor="#003da5" />
        <stop offset={1} stopColor="#009cff" />
      </linearGradient>
    </defs>
    <circle cx={16} cy={16} r={16} fill="url(#linear-gradient)" />
    <path className="cls-2" d="M15 9h2v14h-2z" />
    <path className="cls-2" transform="rotate(-90 16 16)" d="M15 9h2v14h-2z" />
  </svg>
);

export default SvgBotPlusIcon;
