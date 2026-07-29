import "./Logo.css";

interface LogoProps {
  size?: number;
}

const Logo = ({ size = 28 }: LogoProps) => (
  <img
    src="/logo.svg"
    alt="Gymboo"
    className="logo-mark"
    width={size}
    height={size}
  />
);

export default Logo;