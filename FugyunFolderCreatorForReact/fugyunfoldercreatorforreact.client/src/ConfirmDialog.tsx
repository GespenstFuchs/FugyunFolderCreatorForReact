import React from "react";
import './ConfirmDialog.css';

interface ConfirmDialogProps {
    message: string;
    onConfirm: () => void;
    onCancel: () => void;
    iconSrc?: string;
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({ message, onConfirm, onCancel, iconSrc }) => {
    return (
        <div className="overlay">
            <div className="dialog">
                <div className="content">
                    {iconSrc && <img src={iconSrc} alt="Icon" className="icon" />}
                    <p className="message">{message}</p>
                </div>
                <div className="button-group">
                    <button className="yes-button" onClick={onConfirm}>はい</button>
                    <button className="no-button" onClick={onCancel}>いいえ</button>
                </div>
            </div>
        </div>
    );
};

export default ConfirmDialog;
