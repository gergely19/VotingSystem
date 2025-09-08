﻿import Form from "react-bootstrap/Form";
import { SubmitButton } from "@/components/buttons/SubmitButton";
import { FormError } from "@/components/FormError";
import { useState } from "react";
import { yupErrorsToObject } from "@/utils/forms";
import * as yup from "yup";
import { ErrorAlert } from "@/components/alerts/ErrorAlert";
import { useNavigate } from "react-router-dom";
import { createUser } from "@/api/client/users-client";
import { ChangeEvent, FormEvent } from "react";
import { phoneRegex } from "@/utils/regex";
import {ServerSideValidationError} from "@/api/errors/ServerSideValidationError";
import {HttpError} from "@/api/errors/HttpError";

// Use a different model than the dto for register,
// because the form needs an additional field to confirm the password
interface RegisterFormData {
    name: string;
    email: string;
    password: string;
    confirmPassword: string;
}

const registerFormValidator = yup.object({
    name: yup.string().required("Név megadása kötelező"),
    email: yup.string().email("Helytelen email").required("Email megadása kötelező"),
    password: yup.string().required("Jelszó megadása kötelező"),
    confirmPassword: yup.string()
        .oneOf([yup.ref("password"), ""], "Jelszavaknak meg kell egyeznie")
        .required("Jelszó megerősítése kötelező"),
});

export function RegisterPage() {
    const navigate = useNavigate();
    const [loading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [registerData, setRegisterData] = useState<RegisterFormData>({
        name: "",
        email: "",
        password: "",
        confirmPassword: "",
    });
    const [formErrors, setFormErrors] = useState<Record<string, string>>({});

    function handleInputChange(e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
        setRegisterData(prevState => ({
            ...prevState,
            [e.target.name]: e.target.value,
        }));
    }

    async function handleFormSubmit(evt: FormEvent<HTMLFormElement>) {
        evt.preventDefault();

        setError(null);
        setFormErrors({});
        setIsLoading(true);

        try {
            await registerFormValidator.validate(registerData, { abortEarly: false });
            await createUser({
                name: registerData.name,
                email: registerData.email,
                password: registerData.password,
            });
            navigate("/user/login");
        } catch (e) {
            if (e instanceof yup.ValidationError) {
                setFormErrors(yupErrorsToObject(e.inner));
            } else if (e instanceof ServerSideValidationError) {
                setFormErrors(e.validationErrors);
            } else if (e instanceof HttpError) {
                if (e.status=== 409) {
                    setFormErrors({ email: "Ez az email cím már foglalt" });
                } else {
                    setError(e.message);
            }
                
            } else {
                setError("Unknown error")
            }
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <>
            {error ? <ErrorAlert message={error} /> : null}
            <h1>Regisztráció</h1>
            {/* Disable default HTML validation, because we use Yup */}
            <Form onSubmit={handleFormSubmit} validated={false}>
                <Form.Group className="mb-3">
                    <Form.Label htmlFor="name">Név:</Form.Label>
                    <Form.Control
                        type="text"
                        className="form-control"
                        id="name"
                        name="name"
                        onChange={handleInputChange}
                        value={registerData.name}
                    />
                    <FormError message={formErrors.name}/>
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label htmlFor="email">Email:</Form.Label>
                    <Form.Control
                        type="email"
                        className="form-control"
                        id="email"
                        name="email"
                        onChange={handleInputChange}
                        value={registerData.email}
                    />
                    <FormError message={formErrors.email}/>
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label htmlFor="password">Jelszó:</Form.Label>
                    <Form.Control
                        type="password"
                        className="form-control"
                        id="password"
                        name="password"
                        onChange={handleInputChange}
                        value={registerData.password}
                    />
                    <FormError message={formErrors.password}/>
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label htmlFor="confirmPassword" className="form-label">Jelszó megerősítése:</Form.Label>
                    <Form.Control
                        type="password"
                        className="form-control"
                        id="confirmPassword"
                        name="confirmPassword"
                        onChange={handleInputChange}
                        value={registerData.confirmPassword}
                    />
                    <FormError message={formErrors.confirmPassword} />
                </Form.Group>

                <SubmitButton text="Regisztráció" loading={loading}/>
            </Form>
        </>
    )
}