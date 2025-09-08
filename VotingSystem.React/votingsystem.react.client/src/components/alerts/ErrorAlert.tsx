import Alert from "react-bootstrap/Alert";

interface Props {
    message: string
};

export function ErrorAlert({ message }: Props) {
    return (
        <Alert variant="danger">
            <Alert.Heading>Hiba</Alert.Heading>
            <p>{message}</p>
        </Alert>
    );
}