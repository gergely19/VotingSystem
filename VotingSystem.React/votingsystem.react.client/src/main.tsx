import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.css';
import '@/index.css';

import { RootLayout } from "@/pages/RootLayout";
import { LoginPage } from "@/pages/user/LoginPage";
import { LogoutPage } from "@/pages/user/LogoutPage";
import { RegisterPage } from "@/pages/user/RegisterPage";
import { NotFoundPage } from "@/pages/NotFoundPage";
import { PollsPage } from "@/pages/polls/ActivePollsPage";
import { PollPage } from "@/pages/polls/ActivePollPage";
import { ClosedPollsPage } from "@/pages/polls/ClosedPollsPage";
import { ClosedPollPage } from "@/pages/polls/ClosedPollPage";
import { UserContextProvider } from "@/contexts/UserContextProvider";
import { Protected } from "@/components/Protected";
import { Navigate } from "react-router-dom";


const router = createBrowserRouter([
    {
        element: <RootLayout />,
        children: [
            {
                path: "/",
                element: <Navigate to="/polls" replace />,
            },
            {
                path: "/polls",
                element: <PollsPage />,
            },
            {
                path: "/polls/:pollId",
                element: <PollPage />,
            },
            {
                path: "/closedpolls",
                element: <ClosedPollsPage />,
            },
            {
                path: "/closedpolls/:pollId",
                element: <ClosedPollPage />,
            },
            {
                path: "/user/login",
                element: <LoginPage />,
            },
            {
                path: "/user/logout",
                element: <Protected><LogoutPage /></Protected>,
            },
            {
                path: "/user/register",
                element: <RegisterPage />,
            },
            {
                path: "*",
                element: <NotFoundPage />
            },
        ]
    },
]);

createRoot(document.getElementById('root')!).render(
    <UserContextProvider>
        <RouterProvider router={router} />
    </UserContextProvider>
);
