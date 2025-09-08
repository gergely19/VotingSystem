import { HeaderLink } from "@/components/header/HeaderLink";
import { useUserContext } from "@/contexts/UserContext";
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';

export function Header() {
    const { loggedIn } = useUserContext();
    
    return (
        <Navbar variant="light" bg="light" expand="md">
            <Container>
                <Navbar.Brand>Anonim szavazó rendszer</Navbar.Brand>
                <Navbar.Toggle aria-controls="header-navbar-nav" />
                <Navbar.Collapse id="header-navbar-nav">
                    <Nav className="me-auto">
                        {/* <HeaderLink to="/" text="Home"/> */}
                        {/*<HeaderLink to="/movies" text="Movies"/>*/}
                        {loggedIn ? <HeaderLink to="/polls" text="Aktív szavazások" /> : null}
                        {loggedIn ? <HeaderLink to="/closedpolls" text="Lezárult szavazások" /> : null}
                    </Nav>
                    <Nav className="ms-auto">
                        {loggedIn
                            ? <HeaderLink to="/user/logout" text="Kijelentkezés" />
                            : (<>
                                <HeaderLink to="/user/login" text="Bejelentkezés"/>
                                <HeaderLink to="/user/register" text="Regisztráció"/>
                            </>)  }
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
}