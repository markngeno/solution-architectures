import React from "react";
import styled from "styled-components";
import classNames from "classnames";

const Styled = styled.div`
  width: 10px;
  height: 10px;

  .icon-background {
    fill: #ffffff;
  }
  .icon-foreground.outer {
    fill: #8a8d90;
  }
  .icon-foreground.inner {
    fill: #ffffff;
  }

  &.selected {
    .icon-foreground.inner {
      fill: #003da5;
    }
  }
`;

const IconBullet = props => {
  const classes = classNames({
    "icon-bullet": true,
    selected: props.selected
  });

  return (
    <Styled className={classes}>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
        <g>
          <ellipse
            transform="matrix(0.2672 -0.9636 0.9636 0.2672 -2.3096 16.9647)"
            className="icon-background"
            cx="10"
            cy="10"
            rx="9.6"
            ry="9.6"
          />
          <path
            className="icon-foreground outer"
            d="M10,20c-0.9,0-1.8-0.1-2.7-0.4c-2.6-0.7-4.7-2.4-6-4.7c-1.3-2.3-1.6-5-0.9-7.6c0.7-2.6,2.4-4.7,4.7-6
            c2.3-1.3,5-1.6,7.6-0.9l0,0c2.6,0.7,4.7,2.4,6,4.7c1.3,2.3,1.6,5,0.9,7.6C18.4,17.1,14.4,20,10,20z M10,0.8
            C8.4,0.8,6.9,1.2,5.5,2C3.3,3.2,1.8,5.2,1.1,7.5c-1.4,4.9,1.5,10,6.4,11.3c4.9,1.4,10-1.5,11.3-6.4c0.7-2.4,0.4-4.8-0.9-7
            c-1.2-2.1-3.2-3.7-5.5-4.3l0,0C11.6,0.9,10.8,0.8,10,0.8z"
          />
          <ellipse
            transform="matrix(0.2672 -0.9636 0.9636 0.2672 -2.3096 16.9647)"
            className="icon-foreground inner"
            cx="10"
            cy="10"
            rx="6"
            ry="6"
          />
        </g>
      </svg>
    </Styled>
  );
};

export default IconBullet;
