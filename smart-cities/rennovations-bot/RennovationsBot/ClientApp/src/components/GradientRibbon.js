import styled from "styled-components";

const GradientRibbon = styled.div`
  background: ${props => props.theme.blue_gradient};
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 6px;
`;

export default GradientRibbon;
