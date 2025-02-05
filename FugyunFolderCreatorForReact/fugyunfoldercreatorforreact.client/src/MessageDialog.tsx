import React from "react";
import "./MessageDialog.css";

interface MessageDialogProps {
    message: string;
    onClose: () => void;
    iconSrc?: string;
}

const MessageDialog: React.FC<MessageDialogProps> = ({ message, onClose, iconSrc }) => {
    return (
        <div className="overlay">
            <div className="dialog">
                <div className="content">
                    {iconSrc && <img src={iconSrc} alt="Icon" className="icon" />}
                    <p className="message">{message}</p>
                </div>
                <div className="button-group">
                    <button className="yes-button" onClick={onClose}>ＯＫ</button>
                </div>
            </div>
        </div>
    );
};

export default MessageDialog;
